namespace Bookshelf.Resources;

public static class FlareSolverrResource
{
    public const string FlareSolverrImage = "flaresolverr/flaresolverr";
    public const string FlareSolverrRegistry = "ghcr.io";
    public const string FlareSolverrTag = "latest";
    public const int FlareSolverrDefaultPort = 8191;
    
    public static IResourceBuilder<ContainerResource> AddFlareSolverr(this IDistributedApplicationBuilder builder, string name, int? port = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = builder.AddContainer(name, FlareSolverrImage, FlareSolverrTag).WithImageRegistry(FlareSolverrRegistry)
            .WithHttpEndpoint(port ?? FlareSolverrDefaultPort, FlareSolverrDefaultPort, "http");
        
        return resource;
    }
}