namespace Bookshelf.Api.Config;

public class FlareSolverrConfig
{
    public required string ServerUrl { get; set; }
    public int MaxTimeout { get; set; } = 6000;
    public string? ProxyUrl { get; set; }
    public string? ProxyUsername { get; set; }
    public string? ProxyPassword { get; set; }
}