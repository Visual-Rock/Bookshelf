using Bookshelf.DataModel;

namespace Bookshelf.Api.Dto;

public class BookDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
    public string? PublisherId { get; set; }
    public int Pages { get; set; }
    public string Language { get; set; } = string.Empty;

    public string[] Authors { get; set; } = [];
    public string[] Categories { get; set; } = [];
}

internal static class BookDtoExtensions
{
    public static BookDto ToDto(this Book book)
    {
        return new BookDto()
        {
            Id = book.Id,
            Title = book.Title,
            Description = book.Description,
            Isbn = book.Isbn,
            PublishDate = book.PublishDate,
            PublisherId = book.PublisherId?.ToString(),
            Pages = book.Pages,
            Language = book.Language,
            Authors = book.Authors.Select(a => a.Name).ToArray(),
            Categories = book.Categories.Select(c => c.Name).ToArray()
        };
    }
    
}