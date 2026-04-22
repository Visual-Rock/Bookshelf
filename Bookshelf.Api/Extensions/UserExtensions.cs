using System.Security.Claims;
using Bookshelf.Api.Services;
using Bookshelf.DataModel;

namespace Bookshelf.Api.Extensions;

public static class UserExtensions
{
    public static User? GetUser(this ClaimsPrincipal user, IUserService userService)
    {
        if (!user.Identity?.IsAuthenticated ?? true)
            return null;
        return userService.GetUser(user.FindFirst("sub")?.Value ?? string.Empty);
    }
}