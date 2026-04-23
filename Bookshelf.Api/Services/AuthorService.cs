using Bookshelf.DataAccess;
using Bookshelf.DataModel;

namespace Bookshelf.Api.Services;

public interface IAuthorService
{
    Author GetOrCreate(string name);
}

public class AuthorService(BookshelfContext context) : IAuthorService
{
    public Author GetOrCreate(string name)
    {
        var author = context.Authors.FirstOrDefault(a => a.Name == name);

        if (author is null)
        {
            author = new Author()
            {
                Id = Guid.CreateVersion7(),
                Name = name
            };

            context.Add(author);
            context.SaveChanges();
        }

        return author;
    }
}