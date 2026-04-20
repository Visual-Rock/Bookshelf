namespace Bookshelf.DataModel;

public class Book : BaseObject
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Isbn { get; set; }
    public DateTime PublishDate { get; set; }
    public Guid? PublisherId { get; set; }
    public int Pages { get; set; }
    public required string Language { get; set; }
    public required string ExternalId { get; set; }
    
    public Publisher? Publisher { get; set; }
    public IList<Author> Authors { get; set; } = [];
    public IList<Category> Categories { get; set; } = [];
    public IList<UserBookRelation> UserBookRelations { get; set; } = [];
}