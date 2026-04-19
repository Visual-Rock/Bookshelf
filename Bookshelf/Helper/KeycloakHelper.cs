using Bookshelf.Extensions;

namespace Bookshelf.Helper;

public static class KeycloakHelper
{
    private const string BookshelfClientSecret = "23OfASP5ir7cPaMx5K3ozKP0tRQAL7Bp";

    public static void ConfigureKeycloak(IResourceBuilder<KeycloakResource> keycloak,
        IResourceBuilder<IResourceWithEndpoints> api, IResourceBuilder<IResourceWithEndpoints> ui)
    {
        var realm = keycloak.AddRealm("bookshelf");

        var url = api.GetEndpoint().EndpointAnnotation.GetUrl();
        var client = realm.AddClient("bookshelf", "bookshelf")
            .WithClientType(KeycloakClientType.OpenIdConnect)
            .WithStandardFlow()
            .WithClientAuthentication()
            .WithRedirectUris($"{url}/signin-oidc")
            .WithWebOrigins($"{url}", ui.GetEndpoint().EndpointAnnotation.GetUrl())
            .WithClientSecret(BookshelfClientSecret);

        var user = realm.AddUser("user", "user@example.com", "User", "user", "user");
    }

    public static void AddKeycloakEnvironment(IResourceBuilder<KeycloakResource> keycloak,
        IResourceBuilder<IResourceWithEnvironment> resource)
    {
        var kc = keycloak.GetEndpoint();
        resource.WithEnvironment("Authentication:Authority", $"{kc.EndpointAnnotation.GetUrl()}/realms/bookshelf");
        resource.WithEnvironment("Authentication:ClientId", "bookshelf");
        resource.WithEnvironment("Authentication:ClientSecret", BookshelfClientSecret);
        resource.WithEnvironment("Authentication:CallbackPath", "/signin-oidc");
        resource.WithEnvironment("Authentication:RequireHttpsMetadata", kc.IsHttps.ToString());
    }
}