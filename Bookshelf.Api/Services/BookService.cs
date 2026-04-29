using System.Diagnostics.CodeAnalysis;
using Bookshelf.DataAccess;
using Bookshelf.DataModel;
using Microsoft.EntityFrameworkCore;

namespace Bookshelf.Api.Services;

public interface IBookService
{
    void AddBook(Book book);
    Book? GetBook(Guid id);
    IEnumerable<Book> GetBooksForUser(User user);
    Task<Book?> GetBookByIsbn(string isbn);
    void AddOrIncrementBookForUser(Book book, User user);
    void RemoveBooksForUser(Book book, User user, int amount);
}

public class BookService(BookshelfContext context, IServiceProvider serviceProvider, ILogger<BookService> logger) : IBookService
{
    public void AddBook(Book book)
    {
        context.Add(book);
        context.SaveChanges();
    }

    public Book? GetBook(Guid id)
    {
        return context.Books.FirstOrDefault(x => x.Id == id);
    }

    public IEnumerable<Book> GetBooksForUser(User user)
    {
        return context.Books.Include(b => b.Authors).Include(x => x.Categories).Include(b => b.UserBookRelations).ThenInclude(r => r.User).Where(x => x.UserBookRelations.Any(y => y.UserId == user.Id));
    }

    public async Task<Book?> GetBookByIsbn(string isbn)
    {
        var book = context.Books.Include(b => b.Authors).Include(x => x.Categories).Include(b => b.UserBookRelations).ThenInclude(r => r.User).FirstOrDefault(x => x.Isbn == isbn);
        
        if (book is null)
        {
            var external = serviceProvider.GetServices<IExternalBookService>();

            foreach (var service in external)
            {
                try
                {
                    book = await service.GetBookFromIsbn(isbn, true);
                    if (book is not null)
                    {
                        context.Books.Add(book);
                        await context.SaveChangesAsync();
                        break;
                    }
                }
                catch (Exception e)
                {
                    logger.LogError("failed to get book from external provider: {e}", e);
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
    
    public void RemoveBooksForUser(Book book, User user, int amount)
    {
        var relation = context.UserBookRelations.FirstOrDefault(x => x.BookId == book.Id && x.UserId == user.Id);
        if (relation is null) return;
        
        relation.Count -= amount;
        if (relation.Count <= 0)
            context.UserBookRelations.Remove(relation);
        context.SaveChanges();
    }
}