using Bookshelf.Api.Extensions;
using Bookshelf.Api.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace Bookshelf.Api.Controllers;

[ApiController]
[Route("user")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet]
    [Route("login")]
    public IActionResult LoginOidc([FromQuery] string redirectUri = "/")
    {
        return Challenge(new AuthenticationProperties { RedirectUri = redirectUri },
            OpenIdConnectDefaults.AuthenticationScheme);
    }

    [HttpGet]
    [Route("info")]
    public IActionResult GetInfo()
    {
        if (User.GetUser(userService) is not { } user)
            return Unauthorized();
        return Ok(new { user.Id, user.Username });
    }

    [HttpGet]
    [Route("logout")]
    public IActionResult Logout([FromQuery] string redirectUri)
    {
        return SignOut(new AuthenticationProperties { RedirectUri = redirectUri },
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme);
    }
}