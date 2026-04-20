namespace Bookshelf.DataModel;

public class Category : BaseObject
{
    public required string Name { get; set; }

    public IList<Book> Books { get; set; } = [];
}