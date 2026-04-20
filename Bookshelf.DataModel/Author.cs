namespace Bookshelf.DataModel;

public class Author : BaseObject
{
    public required string Name { get; set; }

    public IList<Book> Books { get; set; } = [];
}