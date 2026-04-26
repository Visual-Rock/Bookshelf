using System.Diagnostics.CodeAnalysis;
using Bookshelf.DataAccess;
using Bookshelf.DataModel;
using Microsoft.EntityFrameworkCore;

namespace Bookshelf.Api.Services;

public interface IBookService
{
    void AddBook(Book book);
    Task<Book?> GetBookByIsbn(string isbn);
    void AddOrIncrementBookForUser(Book book, User user);
}

public class BookService(BookshelfContext context, IServiceProvider serviceProvider) : IBookService
{
    public void AddBook(Book book)
    {
        context.Add(book);
        context.SaveChanges();
    }

    public async Task<Book?> GetBookByIsbn(string isbn)
    {
        var book = context.Books.Include(b => b.Authors).Include(x => x.Categories).FirstOrDefault(x => x.Isbn == isbn);
        
        if (book is null)
        {
            var external = serviceProvider.GetServices<IExternalBookService>();

            foreach (var service in external)
            {
                book = await service.GetBookFromIsbn(isbn, true);
                if (book is not null)
                {
                    context.Books.Add(book);
                    await context.SaveChangesAsync();
                    break;
                }
            }
        }
        
        return book;
    }

    public void AddOrIncrementBookForUser(Book book, User user)
    {
        var relation = context.UserBookRelations.FirstOrDefault(x => x.BookId == book.Id && x.UserId == user.Id);
        if (relation is null)
        {
            relation = new UserBookRelation { Id = Guid.CreateVersion7(), Book = book, User = user, Count = 0 };
            context.Add(relation);
        }
        
        relation.Count += 1;
        context.SaveChanges();
    }
}