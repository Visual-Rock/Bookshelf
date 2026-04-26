namespace Bookshelf.Api.Services;

public interface IBookCoverService
{
    Task DownloadCover(Guid id, string url, bool isThumbnail);
    (string Path, string ContentType)? GetCover(Guid id);
    (string Path, string ContentType)? GetThumbnail(Guid id);
} 

public class BookCoverService(IConfiguration configuration) : IBookCoverService
{
    private static readonly HttpClient Client = new();
    private static readonly string JpegContentType = "image/jpeg"; 
    private readonly string _imageDirectory = configuration["Storage:ImageDirectory"] ?? "images";
    
    public async Task DownloadCover(Guid id, string url, bool isThumbnail)
    {
        var baseDir = Path.Join(_imageDirectory, id.ToString());
        if (!Directory.Exists(baseDir))
            Directory.CreateDirectory(baseDir);

        var path = Path.Join(baseDir, $"{(isThumbnail ? "thumbnail" : "cover")}.jpeg");

        await using var data = await Client.GetStreamAsync(url);
        await using var file = File.OpenWrite(path);
        await data.CopyToAsync(file);
        file.Close();
    }

    public (string Path, string ContentType)? GetCover(Guid id)
    {
        var path = Path.GetFullPath(Path.Join(_imageDirectory, id.ToString(), "cover.jpeg"));
        return File.Exists(path) ? (path, JpegContentType) : null;
    }

    public (string Path, string ContentType)? GetThumbnail(Guid id)
    {
        var path = Path.GetFullPath(Path.Join(_imageDirectory, id.ToString(), "thumbnail.jpeg"));
        return File.Exists(path) ? (path, JpegContentType) : GetCover(id);
    }
}