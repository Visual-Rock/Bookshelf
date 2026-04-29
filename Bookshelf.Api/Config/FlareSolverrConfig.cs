namespace Bookshelf.Api.Config;

public class FlareSolverrConfig
{
    public required string ServerUrl { get; set; }
    public int MaxTimeout { get; set; } = 6000;
}