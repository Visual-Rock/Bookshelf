namespace Bookshelf.DataModel;

public class User : BaseObject
{
    public required string Username { get; set; }
    public required string ExternalReference { get; set; }
}