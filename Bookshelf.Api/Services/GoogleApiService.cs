using System.Text.Json;
using Bookshelf.DataModel;

namespace Bookshelf.Api.Services;

public class GoogleApiService(IConfiguration configuration, IAuthorService authorService, ICategoryService categoryService, IPublisherService publisherService) : IExternalBookService
{
    private readonly HttpClient _client = new() { BaseAddress = new Uri("https://www.googleapis.com") };
    private readonly string? _apiKey = configuration["GoogleApi:ApiKey"];
    private readonly string _imageDirectory = configuration["Storage:ImageDirectory"] ?? "images";
    
    public async Task<Book?> GetBookFromIsbn(string isbn, bool saveThumbnails = false)
    {
        var (count, books) = await SearchBooks(isbn);

        if (count == 0)
            return null;

        var candidate = books!.Value.EnumerateArray().First();
        var id = candidate.GetProperty("id").GetString();

        if (id is null)
            return null;
        
        var bookData = JsonDocument.Parse(await _client.GetStringAsync(PrepUrl($"/volumes/{id}"))).RootElement;
        var volumeInfo = bookData.GetProperty("volumeInfo");

        var title = volumeInfo.GetProperty("title").GetString() ?? string.Empty;
        var description = volumeInfo.GetProperty("description").GetString() ?? string.Empty;
        var language = volumeInfo.GetProperty("language").GetString() ?? string.Empty;
        var pageCount = volumeInfo.GetProperty("pageCount").GetInt32();
        var publishedDate = volumeInfo.GetProperty("publishedDate").GetString() ?? "0001-01-01";
        var publisher = volumeInfo.GetProperty("publisher").GetString();

        var authors = volumeInfo.GetProperty("authors").EnumerateArray().Select(a => authorService.GetOrCreate(a.GetString()!));
        var categories = volumeInfo.GetProperty("categories").EnumerateArray().SelectMany(c => c.GetString()!.Split("/").Select(x => x.Trim()));
        
        var book = new Book
        {
            Id = Guid.CreateVersion7(),
            Title = title,
            Description = description,
            Isbn = isbn,
            Language = language,
            ExternalId = id,
            Pages = pageCount,
            Publisher = publisher is not null ? publisherService.GetOrCreate(publisher) : null,
            PublishDate = new DateTime(DateOnly.Parse(publishedDate), TimeOnly.MinValue, DateTimeKind.Utc),
            Authors = authors.ToList(),
            Categories = categories.Select(categoryService.GetOrCreate).ToList()
        };

        await DownloadImages(book.Id, volumeInfo.GetProperty("imageLinks"));

        return book;
    }

    private async Task DownloadImages(Guid bookId, JsonElement links)
    {
        var thumbnailLink = links.GetProperty("thumbnail").GetString() ?? string.Empty;
        var extraLargeLink = links.GetProperty("extraLarge").GetString() ?? string.Empty;

        var baseDir = Path.Join(_imageDirectory, bookId.ToString());

        if (!Directory.Exists(baseDir))
            Directory.CreateDirectory(baseDir);

        await Task.WhenAll(Download(thumbnailLink, Path.Join(baseDir, "thumbnail.jpeg")), Download(extraLargeLink, Path.Join(baseDir, "cover.jpeg")));
        
        async Task Download(string? url, string path)
        {
            if (url is null)
                return;

            await using var data = await _client.GetStreamAsync(url);
            await using var file = File.OpenWrite(path);
            await data.CopyToAsync(file);
            file.Close();
        }
    }
    

    private async Task<(int Count, JsonElement?)> SearchBooks(string query)
    {
        var result = JsonDocument.Parse(await _client.GetStringAsync(PrepUrl($"/volumes?q={query}", true))).RootElement;
        result.TryGetProperty("items", out var items);
        return (result.GetProperty("totalItems").GetInt32(), items);
    }

    private string PrepUrl(string url, bool hasQuery = false)
    {
        var keySegment = (hasQuery, !string.IsNullOrEmpty(_apiKey)) switch
        {
            (_, false) => string.Empty,
            (true, true) => $"&key={_apiKey}",
            (false, true) => $"?key={_apiKey}",
        };

        return $"/books/v1{url}{keySegment}";
    }
}