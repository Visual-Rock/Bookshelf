using Bookshelf.DataAccess.Extensions;
using Bookshelf.DataModel;
using Microsoft.EntityFrameworkCore;

namespace Bookshelf.DataAccess;

public class BookshelfContext(DbContextOptions<BookshelfContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<UserBookRelation> UserBookRelations { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>(b =>
        {
            b.ToTable("users");
            b.HasKey(u => u.Id).HasName("pk_user");

            b.Property(u => u.Id).HasColumnName("id");
            b.Property(u => u.Username).HasColumnName("username").HasMaxLength(32);
            b.Property(u => u.ExternalReference).HasColumnName("external_reference");
            b.Property(u => u.IsShelfPublic).HasColumnName("is_shelf_public");

            b.HasIndex(u => u.Username, "ix_username").IsUnique();
        });

        builder.Entity<Book>(b =>
        {
            b.ToTable("books");
            b.HasKey(x => x.Id).HasName("pk_book");

            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.Title).HasColumnName("title");
            b.Property(x => x.Description).HasColumnName("description");
            b.Property(x => x.Isbn).HasColumnName("isbn");
            b.Property(x => x.PublishDate).HasColumnName("publish_date");
            b.Property(x => x.PublisherId).HasColumnName("publisher_id").IsRequired(false);
            b.Property(x => x.Pages).HasColumnName("pages");
            b.Property(x => x.Language).HasColumnName("language");
            b.Property(x => x.ExternalId).HasColumnName("external_id");

            b.HasOne(x => x.Publisher).WithMany(x => x.Books).HasForeignKey(x => x.PublisherId).HasConstraintName("fk_book_publisher").OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<Author>(b =>
        {
            b.ToTable("authors");
            b.HasKey(x => x.Id).HasName("pk_author");

            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.Name).HasColumnName("name");
        });

        builder.Entity<Publisher>(b =>
        {
            b.ToTable("publishers");
            b.HasKey(x => x.Id).HasName("pk_publisher");

            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.Name).HasColumnName("name");
        });

        builder.Entity<Category>(b =>
        {
            b.ToTable("categories");
            b.HasKey(x => x.Id).HasName("pk_category");

            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.Name).HasColumnName("name");
        });

        builder.Entity<UserBookRelation>(b =>
        {
            b.ToTable("user_book_relations");
            b.HasKey(x => x.Id).HasName("pk_user_book_relation");

            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.Count).HasColumnName("count");
            b.Property(x => x.BookId).HasColumnName("book_id");
            b.Property(x => x.UserId).HasColumnName("user_id");

            b.HasOne(x => x.Book).WithMany(x => x.UserBookRelations).HasForeignKey(x => x.BookId).HasConstraintName("fk_user_book_relation_book");
            b.HasOne(x => x.User).WithMany(x => x.UserBookRelations).HasForeignKey(x => x.UserId).HasConstraintName("fk_user_book_relation_user");
        });
        
        builder.ConfigureManyToMany<Book, Author>(b => b.Authors, a => a.Books, "book", "author");
        builder.ConfigureManyToMany<Book, Category>(b => b.Categories, a => a.Books, "book", "category");
        
        base.OnModelCreating(builder);
    }
}