using Bookshelf.Api.Dto;
using Bookshelf.Api.Extensions;
using Bookshelf.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bookshelf.Api.Controllers;

[ApiController]
[Route("library")]
public class LibraryController(IUserService userService, ILibraryService libraryService, IBookService bookService) : ControllerBase
{
    [HttpGet("public")]
    public IActionResult GetPublicLibraries()
    {
        if (User.GetUser(userService) is not { } user)
            return Unauthorized();
        return Ok(libraryService.GetPublicLibraries().ToDtoList(user));
    }
    
    [HttpGet("list")]
    public IActionResult GetLibrary([FromQuery] Guid? userId)
    {
        if (User.GetUser(userService) is not { } user)
            return Unauthorized();

        var libraryUser = user;
        if (userId.HasValue && user.Id != userId.Value)
        {
            var u = userService.GetUser(userId.Value);
            if (u is null || !u.IsShelfPublic)
                return NotFound();
            libraryUser = u;
        }
        
        return Ok(bookService.GetBooksForUser(libraryUser).OrderBy(b => b.Title).Select(x => x.ToDto(libraryUser)));
    }
}