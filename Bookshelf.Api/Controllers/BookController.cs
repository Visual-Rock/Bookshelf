using Bookshelf.Api.Dto;
using Bookshelf.Api.Extensions;
using Bookshelf.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bookshelf.Api.Controllers;

public record struct BookIsbnBody(string Isbn);

[ApiController]
[Route("book")]
public class BookController(IUserService userService, IIsbnService isbnService, IBookService bookService, IGoogleApiService googleApiService) : ControllerBase
{
    [HttpPost("add")]
    public async Task<IActionResult> AddBook([FromBody] BookIsbnBody body)
    {
        if (User.GetUser(userService) is not { } user)
            return Unauthorized();

        if (isbnService.FormatIsbn(body.Isbn) is not { } isbn)
            return BadRequest();

        if (!bookService.TryGetBookByIsbn(isbn, out var book))
        {
            book = await googleApiService.GetBookFromIsbn(isbn, true);

            if (book is null)
                return NotFound("book could not be found");
            bookService.AddBook(book);
        }
        
        bookService.AddOrIncrementBookForUser(book, user);
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetBookByIsbn([FromQuery(Name = "isbn")] string query)
    {
        if (User.GetUser(userService) is not { } user)
            return Unauthorized();

        if (isbnService.FormatIsbn(query) is not { } isbn)
            return BadRequest();

        if (!bookService.TryGetBookByIsbn(isbn, out var book))
        {
            book = await googleApiService.GetBookFromIsbn(isbn, true);

            if (book is null)
                return NotFound("book could not be found");
            bookService.AddBook(book);
        }
        
        return Ok(book.ToDto());
    }
}