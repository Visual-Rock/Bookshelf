namespace Bookshelf.Extensions;

public static class ResourceExtensions
{
    public static EndpointReference GetEndpoint<T>(this IResourceBuilder<T> project) where T : IResourceWithEndpoints
    {
        if (project.GetEndpoint("https") is { Exists: true } https)
            return https;
        return project.GetEndpoint("http");
    }

    public static string GetUrl(this EndpointAnnotation endpoint)
    {
        return $"{endpoint.Transport}://{endpoint.TargetHost}:{endpoint.Port}";
    }
}