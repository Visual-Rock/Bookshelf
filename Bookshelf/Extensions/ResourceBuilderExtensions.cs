namespace Aurora.AppHost.Extensions;

public static class ResourceBuilderExtensions
{
    public static IResourceBuilder<T> WithApiReference<T>(this IResourceBuilder<T> builder) where T : IResourceWithEndpoints
    {
        AddUrl(builder.GetEndpoint("http"), "Api (http)");
        AddUrl(builder.GetEndpoint("https"), "Api (https)");

        return builder;

        void AddUrl(EndpointReference endPoint, string name)
        {
            if (endPoint.Exists)
                builder.WithUrl($"{endPoint}/scalar/v1", name);
        }
    }
}