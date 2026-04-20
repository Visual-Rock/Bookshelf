namespace Bookshelf.DataModel;

public class Publisher : BaseObject
{
    public required string Name { get; set; }

    public IList<Book> Books { get; set; } = [];
}