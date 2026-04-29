using System.Text.Json.Serialization;
using Bookshelf.DataModel;

namespace Bookshelf.Api.Dto;

public class BookDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
    public string? Publisher { get; set; }
    public int Pages { get; set; }
    public string Language { get; set; } = string.Empty;
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Amount { get; set; }

    public string[] Authors { get; set; } = [];
    public string[] Categories { get; set; } = [];

    public Dictionary<string, int> PublicUserAmount { get; set; } = [ ];
}

internal static class BookDtoExtensions
{
    public static BookDto ToDto(this Book book)
    {
        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Description = book.Description,
            Isbn = book.Isbn,
            PublishDate = book.PublishDate,
            Publisher = book.Publisher?.Name,
            Pages = book.Pages,
            Language = book.Language,
            Authors = book.Authors.Select(a => a.Name).ToArray(),
            Categories = book.Categories.Select(c => c.Name).ToArray()
        };
    }
    
    public static BookDto ToDto(this Book book, User user)
    {
        var b = book.ToDto();
        if (book.UserBookRelations.FirstOrDefault(x => x.UserId == user.Id) is { } relation)
            b.Amount = relation.Count;
        b.PublicUserAmount = book.UserBookRelations.Where(x => x.User is not null && x.User.IsShelfPublic && x.UserId != user.Id)
                                                   .ToDictionary(x => x.User.Username, x => x.Count);
        return b;
    }
}