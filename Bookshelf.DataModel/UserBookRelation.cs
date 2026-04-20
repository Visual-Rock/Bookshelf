namespace Bookshelf.DataModel;

public class UserBookRelation : BaseObject
{
    public int Count { get; set; }
    public Guid BookId { get; set; }
    public Guid UserId { get; set; }

    public required Book Book { get; set; }
    public required User User { get; set; }
}