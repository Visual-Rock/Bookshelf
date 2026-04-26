using Bookshelf.DataModel;
using HtmlAgilityPack;

namespace Bookshelf.Api.Services;

public class IsbnSearchService(IConfiguration configuration, IAuthorService authorService, IPublisherService publisherService, HttpClient client) : IExternalBookService
{
    private readonly string _imageDirectory = configuration["Storage:ImageDirectory"] ?? "images";

    public async Task<Book?> GetBookFromIsbn(string isbn, bool saveThumbnails = false)
    {
        var url = $"https://isbnsearch.org/isbn/{isbn.Replace("-", "")}";

        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            return null;

        var html = await response.Content.ReadAsStringAsync();
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var bookNode = doc.DocumentNode.SelectSingleNode("//div[@id='book']");
        if (bookNode == null)
            return null;

        var title = bookNode.SelectSingleNode(".//h1")?.InnerText;
        if (string.IsNullOrEmpty(title))
            return null;

        var bookInfo = bookNode.SelectSingleNode(".//div[@class='bookinfo']");

        var authorName = GetValue(bookInfo, "Author") ?? string.Empty;
        var publisherName = GetValue(bookInfo, "Publisher") ?? string.Empty;
        var publishedDateStr = GetValue(bookInfo, "Published") ?? string.Empty;
        
        var publishDate = DateTime.MinValue;
        if (int.TryParse(publishedDateStr, out var year)) 
            publishDate = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var author = authorService.GetOrCreate(authorName);
        var publisher = publisherService.GetOrCreate(publisherName);

        var bookId = Guid.CreateVersion7();

        if (saveThumbnails)
        {
            var imageUrl = bookNode.SelectSingleNode(".//div[@class='image']/img")?.GetAttributeValue("src", null);
            if (!string.IsNullOrEmpty(imageUrl)) 
                await DownloadImage(bookId, imageUrl);
        }

        return new Book
        {
            Id = bookId,
            Title = HtmlEntity.DeEntitize(title).Trim(),
            Description = string.Empty,
            Isbn = isbn,
            Language = string.Empty,
            ExternalId = isbn,
            PublishDate = publishDate,
            PublisherId = publisher.Id,
            Publisher = publisher,
            Authors = [author]
        };
    }

    private string? GetValue(HtmlNode? node, string label)
    {
        if (node == null) return null;
        var p = node.SelectSingleNode($".//p[strong[contains(text(), '{label}:')]]");
        if (p == null) return null;

        var text = p.InnerText;
        var labelIndex = text.IndexOf($"{label}:", StringComparison.Ordinal);
        if (labelIndex == -1) return null;

        return HtmlEntity.DeEntitize(text[(labelIndex + label.Length + 1)..]).Trim();
    }

    private async Task DownloadImage(Guid bookId, string url)
    {
        var baseDir = Path.Join(_imageDirectory, bookId.ToString());

        if (!Directory.Exists(baseDir))
            Directory.CreateDirectory(baseDir);

        var path = Path.Join(baseDir, "cover.jpeg");
        try
        {
            await using var data = await client.GetStreamAsync(url);
            await using var file = File.OpenWrite(path);
            await data.CopyToAsync(file);
        }
        catch
        {
            // Ignore download errors
        }
    }
}