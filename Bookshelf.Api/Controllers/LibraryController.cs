using Bookshelf.Api.Dto;
using Bookshelf.Api.Extensions;
using Bookshelf.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bookshelf.Api.Controllers;

[ApiController]
[Route("library")]
public class LibraryController(IUserService userService, ILibraryService libraryService) : ControllerBase
{
    [HttpGet("public")]
    public IActionResult GetPublicLibraries()
    {
        if (User.GetUser(userService) is not { } user)
            return Unauthorized();
        return Ok(libraryService.GetPublicLibraries().ToDtoList(user));
    }
}