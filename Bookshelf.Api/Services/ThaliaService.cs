using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Text.Json;
using Bookshelf.DataModel;
using HtmlAgilityPack;

namespace Bookshelf.Api.Services;

public class ThaliaService : IExternalBookService
{
    private readonly IAuthorService _authorService;
    private readonly IPublisherService _publisherService;
    private readonly IBookCoverService _bookCoverService;
    private readonly ILogger<ThaliaService> _logger;
    private readonly IFlareSolverrService _flareSolverrService;
    private readonly string _baseUrl = "https://www.thalia.de";

    public ThaliaService(IAuthorService authorService, IPublisherService publisherService, IBookCoverService bookCoverService, ILogger<ThaliaService> logger, IFlareSolverrService flareSolverrService)
    {
        _authorService = authorService;
        _publisherService = publisherService;
        _bookCoverService = bookCoverService;
        _logger = logger;
        _flareSolverrService = flareSolverrService;
    }

    private record BookParseResult(string Name, string Description, DateTime PublishDate, string Publisher, int Pages, string Language, string[] Authors, string ImageUrl);

    public async Task<Book?> GetBookFromIsbn(string isbn, bool saveThumbnails = false)
    {
        try
        {
            if (await SearchThalia(isbn) is not { } searchPage || GetProductUrl(searchPage) is not { } productUrl)
                return null;

            if (await GetProductDetails(productUrl) is not { } productDetails || !TryParseProductDetails(productDetails, out var result))
                return null;

            
            var bookId = Guid.CreateVersion7();
            var publisher = _publisherService.GetOrCreate(result.Publisher);
            var authors = result.Authors.Select(_authorService.GetOrCreate).ToList();
            
            await _bookCoverService.DownloadCover(bookId, result.ImageUrl, false);
            
            return new Book
            {
                Id = bookId,
                Title = result.Name,
                Description = result.Description,
                Isbn = isbn,
                Language = result.Language,
                ExternalId = productUrl,
                PublishDate = result.PublishDate.ToUniversalTime(),
                Pages = result.Pages,
                PublisherId = publisher.Id,
                Publisher = publisher,
                Authors = authors
            };
        }
        catch (Exception e)
        {
            _logger.LogError("failed to get book from thalia {e}", e);
            throw;
        }
    }

    private async Task<string?> SearchThalia(string isbn)
    {
        var url = $"{_baseUrl}/suche?sq={isbn}";
        return await _flareSolverrService.GetStringAsync(url);
    }

    private string? GetProductUrl(string searchResult)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(searchResult);

        var results = doc.DocumentNode.SelectSingleNode("//ul[@class='tm-produktliste']").SelectNodes("li").ToArray();
        if (results.Length != 1)
            return null;
        return results.First().SelectSingleNode("a").Attributes["href"].Value;
    }

    private async Task<string?> GetProductDetails(string url) => await _flareSolverrService.GetStringAsync($"{_baseUrl}{url}");

    private bool TryParseProductDetails(string html, [NotNullWhen(true)] out BookParseResult? result)
    {
        result = null;

        if (string.IsNullOrWhiteSpace(html))
            return false;

        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        
        var name = string.Empty;
        var description = string.Empty;
        var imageUrl = string.Empty;
        string[] authors = [];

        var jsonLdNode = doc.DocumentNode
            .SelectNodes("//script[@type='application/ld+json']")
            .Select(node =>
            {
                try { return JsonDocument.Parse(HtmlEntity.DeEntitize(node.InnerText)); }
                catch (JsonException) { return null; }
            })
            .FirstOrDefault(jsonDoc => jsonDoc?.RootElement.TryGetProperty("@type", out var type) == true && type.GetString() == "Book");

        if (jsonLdNode is not null)
        {
            try
            {
                var root = jsonLdNode.RootElement;

                if (root.TryGetProperty("name", out var nameProp))
                    name = Clean(nameProp.GetString());

                if (root.TryGetProperty("description", out var descProp))
                    description = Clean(descProp.GetString());

                if (root.TryGetProperty("image", out var imagesProp))
                    imageUrl = imagesProp.EnumerateArray().FirstOrDefault().GetString();
                
                if (root.TryGetProperty("author", out var authorProp))
                {
                    authors = authorProp.ValueKind switch
                    {
                        JsonValueKind.Array => authorProp.EnumerateArray().Where(a => a.TryGetProperty("name", out _))
                                                                          .Select(a => Clean(a.GetProperty("name").GetString()))
                                                                          .Where(s => !string.IsNullOrEmpty(s)).ToArray(),
                        JsonValueKind.Object when authorProp.TryGetProperty("name", out var singleName) => [Clean(singleName.GetString())],
                        _ => []
                    };
                }
            }
            catch (JsonException)
            {
                // fall through with whatever was parsed
            }
        }

        var publisher = Clean(FieldValue("Verlag"));

        DateTime publishDate = default;
        var dateRaw = FieldValue("Erscheinungsdatum");
        if (!string.IsNullOrEmpty(dateRaw)) 
            DateTime.TryParseExact(dateRaw, ["dd.MM.yyyy", "d.M.yyyy", "MM/dd/yyyy"], CultureInfo.InvariantCulture, DateTimeStyles.None, out publishDate);

        var pages = 0;
        var pagesRaw = FieldValue("Seitenzahl");
        if (!string.IsNullOrEmpty(pagesRaw))
            int.TryParse(pagesRaw, out pages);

        var language = Clean(FieldValue("Sprache"));

        result = new BookParseResult(name, description, publishDate, publisher, pages, language, authors, imageUrl);
        return true;
        
        string Clean(string? raw) => string.IsNullOrWhiteSpace(raw) ? string.Empty : HtmlEntity.DeEntitize(raw).Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim();

        string? FieldValue(string label)
        {
            var headings = doc.DocumentNode.SelectNodes($"//h3[normalize-space(text())='{label}']");

            if (headings is null) return null;

            foreach (var h3 in headings)
            {
                var sibling = h3.NextSibling;
                while (sibling != null)
                {
                    if (sibling is { NodeType: HtmlNodeType.Element, Name: not ("h3" or "h2" or "h1") })
                    {
                        var val = Clean(sibling.InnerText);
                        if (!string.IsNullOrEmpty(val)) return val;
                    }
                    else if (sibling.NodeType == HtmlNodeType.Text)
                    {
                        var val = sibling.InnerText.Trim();
                        if (!string.IsNullOrEmpty(val)) return val;
                    }

                    sibling = sibling.NextSibling;
                }
            }

            return null;
        }
    }
}