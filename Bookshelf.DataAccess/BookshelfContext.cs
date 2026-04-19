using Bookshelf.DataModel;
using Microsoft.EntityFrameworkCore;

namespace Bookshelf.DataAccess;

public class BookshelfContext(DbContextOptions<BookshelfContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>(b =>
        {
            b.ToTable("users");
            b.HasKey(u => u.Id).HasName("pk_user");

            b.Property(u => u.Id).HasColumnName("id");
            b.Property(u => u.Username).HasColumnName("username").HasMaxLength(32);
            b.Property(u => u.ExternalReference).HasColumnName("external_reference");

            b.HasIndex(u => u.Username, "ix_username").IsUnique();
        });

        base.OnModelCreating(builder);
    }
}