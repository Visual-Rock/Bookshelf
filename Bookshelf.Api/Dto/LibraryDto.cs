using Bookshelf.DataModel;

namespace Bookshelf.Api.Dto;

public class LibraryDto
{
    public required string Username { get; set; }
    public Guid UserId { get; set; }
    public int Books { get; set; }
}

internal static class LibraryDtoExtensions
{
    internal static LibraryDto ToDto(User user, int amount)
    {
        return new LibraryDto { Username = user.Username, UserId = user.Id, Books = amount };
    }

    internal static List<LibraryDto> ToDtoList(this Dictionary<User, int> users, User user)
    {
        return users.Where(x => x.Key.Id != user.Id).Select(x => ToDto(x.Key, x.Value)).ToList();
    }
}