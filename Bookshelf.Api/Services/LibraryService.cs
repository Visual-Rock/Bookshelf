using Bookshelf.DataAccess;
using Bookshelf.DataModel;
using Microsoft.EntityFrameworkCore;

namespace Bookshelf.Api.Services;

public interface ILibraryService
{
    Dictionary<User, int> GetPublicLibraries();
}

public class LibraryService(BookshelfContext context) : ILibraryService
{
    public Dictionary<User, int> GetPublicLibraries()
    {
        return context.Users.Include(u => u.UserBookRelations).Where(u => u.IsShelfPublic)
                            .ToDictionary(x => x, x => x.UserBookRelations.Sum(b => b.Count));
    }
}