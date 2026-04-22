using System.Diagnostics.CodeAnalysis;
using Bookshelf.DataAccess;
using Bookshelf.DataModel;

namespace Bookshelf.Api.Services;

public interface IBookService
{
    void AddBook(Book book);
    bool TryGetBookByIsbn(string isbn, [NotNullWhen(true)] out Book? book);
    void AddOrIncrementBookForUser(Book book, User user);
}

public class BookService(BookshelfContext context) : IBookService
{
    public void AddBook(Book book)
    {
        context.Add(book);
        context.SaveChanges();
    }

    public bool TryGetBookByIsbn(string isbn, [NotNullWhen(true)] out Book? book)
    {
        book = context.Books.FirstOrDefault(x => x.Isbn == isbn);
        return book is not null;
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