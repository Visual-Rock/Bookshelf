using Bookshelf.Api.Dto;
using Bookshelf.Api.Extensions;
using Bookshelf.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bookshelf.Api.Controllers;

public record struct BookIsbnBody(string Isbn);

[ApiController]
[Route("book")]
public class BookController(IUserService userService, IIsbnService isbnService, IBookService bookService) : ControllerBase
{
    [HttpPost("add")]
    public async Task<IActionResult> AddBook([FromBody] BookIsbnBody body)
    {
        if (User.GetUser(userService) is not { } user)
            return Unauthorized();

        if (isbnService.FormatIsbn(body.Isbn) is not { } isbn)
            return BadRequest();

        if (await bookService.GetBookByIsbn(isbn) is not { } book)
            return NotFound();
        
        bookService.AddOrIncrementBookForUser(book, user);
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetBookByIsbn([FromQuery(Name = "isbn")] string query)
    {
        if (User.GetUser(userService) is null)
            return Unauthorized();

        if (isbnService.FormatIsbn(query) is not { } isbn)
            return BadRequest();

        if (await bookService.GetBookByIsbn(isbn) is not { } book)
            return NotFound();
        return Ok(book.ToDto());
    }
}