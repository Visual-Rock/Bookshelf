using Bookshelf.DataModel;

namespace Bookshelf.Api.Services;

public interface IExternalBookService
{
    Task<Book?> GetBookFromIsbn(string isbn, bool saveThumbnails = false);
}