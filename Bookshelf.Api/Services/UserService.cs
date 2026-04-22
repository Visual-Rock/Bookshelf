using Bookshelf.DataAccess;
using Bookshelf.DataModel;
using Microsoft.EntityFrameworkCore;

namespace Bookshelf.Api.Services;

public interface IUserService
{
    User? GetUser(string externalReference);
    Task<User> GetOrCreateUserAsync(string username, string externalReference);
}

public class UserService(BookshelfContext context) : IUserService
{
    public User? GetUser(string externalReference) => context.Users.FirstOrDefault(u => u.ExternalReference == externalReference);

    public async Task<User> GetOrCreateUserAsync(string username, string externalReference)
    {
        var user = await context.Users.FirstOrDefaultAsync(u =>
            u.Username == username && u.ExternalReference == externalReference);

        if (user == null)
        {
            user = new User { Id = Guid.CreateVersion7(), Username = username, ExternalReference = externalReference };
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        return user;
    }
}