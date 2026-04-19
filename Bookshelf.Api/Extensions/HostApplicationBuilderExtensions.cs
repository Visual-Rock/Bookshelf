using System.Security.Claims;
using Bookshelf.Api.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Bookshelf.Api.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddBookshelfServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUserService, UserService>();
        return builder;
    }

    public static void AddBookshelfCors(this IHostApplicationBuilder builder)
    {
        var allowedOrigins =
            (builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string>() ?? string.Empty).Split(',',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(allowedOrigins)
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .WithMethods("GET", "POST", "PUT", "DELETE", "HEAD")
                    .WithExposedHeaders("Content-Range", "Content-Length", "Accept-Ranges", "Content-Disposition");
            });
        });
    }

    public static IHostApplicationBuilder AddBookshelfAuthentication(this IHostApplicationBuilder builder)
    {
        var config = builder.Configuration.GetSection("Authentication");

        builder.Services.AddAuthorization();

        builder.Services.AddAuthentication(o =>
            {
                o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = "bookshelf-auth";
                options.LoginPath = "/user/login";
                options.LogoutPath = "/user/logout";
                options.Cookie.HttpOnly = true;
            })
            .AddOpenIdConnect(o =>
            {
                config.Bind(o);

                o.ResponseType = "code";
                o.SaveTokens = true;
                o.GetClaimsFromUserInfoEndpoint = true;

                o.Events.OnTokenValidated = async context =>
                {
                    var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();

                    var externalReference = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var username = context.Principal?.FindFirst("name")?.Value ??
                                   context.Principal?.FindFirst(ClaimTypes.Name)?.Value ??
                                   externalReference;

                    if (string.IsNullOrEmpty(externalReference))
                    {
                        context.Fail("External Reference (sub) is missing from OIDC provider.");
                        return;
                    }

                    var user = await userService.GetOrCreateUserAsync(username!, externalReference);

                    if (context.Principal?.Identity is ClaimsIdentity identity)
                        identity.AddClaim(new Claim(ClaimTypes.Name, user.Username));
                };
            });

        return builder;
    }
}