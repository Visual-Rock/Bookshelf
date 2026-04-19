using Microsoft.EntityFrameworkCore;

namespace Bookshelf.DataAccess;

public class BookshelfContext(DbContextOptions<BookshelfContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}