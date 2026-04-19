using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace Bookshelf.Api.Controllers;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
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
        if (User.Identity?.IsAuthenticated == true)
        {
            var id = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var username = User.Identity.Name;

            return Ok(new { Id = id, Username = username });
        }

        return Unauthorized();
    }

    [HttpPost]
    [Route("logout")]
    public IActionResult Logout()
    {
        return SignOut(new AuthenticationProperties { RedirectUri = "/" },
            CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
    }
}