using Bookshelf.Extensions;

namespace Bookshelf.Helper;

public static class KeycloakHelper
{
    private const string BookshelfClientSecret = "23OfASP5ir7cPaMx5K3ozKP0tRQAL7Bp";

    public static void ConfigureKeycloak(IResourceBuilder<KeycloakResource> keycloak)
    {
        var realm = keycloak.AddRealm("bookshelf");

        var client = realm.AddClient("bookshelf", "bookshelf")
            .WithClientType(KeycloakClientType.OpenIdConnect)
            .WithStandardFlow()
            .WithClientAuthentication()
            .WithRedirectUris("http://localhost:*/authentication/login-callback")
            .WithWebOrigins("http://localhost:*")
            .WithClientSecret(BookshelfClientSecret);

        var user = realm.AddUser("user", "user@example.com", "User", "", "user");
    }

    public static void AddKeycloakEnvironment(IResourceBuilder<KeycloakResource> keycloak,
        IResourceBuilder<IResourceWithEnvironment> resource)
    {
        var keyCloakEndpoint = keycloak.GetEndpoint();
        resource.WithEnvironment("Authentication:keycloak:Authority",
            $"{keyCloakEndpoint.EndpointAnnotation.GetUrl()}/realms/bookshelf");
        resource.WithEnvironment("Authentication:keycloak:ClientId", "bookshelf");
        resource.WithEnvironment("Authentication:keycloak:ClientSecret", BookshelfClientSecret);
        resource.WithEnvironment("Authentication:keycloak:CallbackPath", "/signin-oidc");
        resource.WithEnvironment("Authentication:keycloak:RequireHttpsMetadata", keyCloakEndpoint.IsHttps.ToString());
    }
}