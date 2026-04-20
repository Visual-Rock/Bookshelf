using System.Linq.Expressions;
using Bookshelf.DataModel;
using Microsoft.EntityFrameworkCore;

namespace Bookshelf.DataAccess.Extensions;

public static class ModelBuilderExtensions
{
    public static void ConfigureManyToMany<TLeft, TRight>(
        this ModelBuilder builder, Expression<Func<TLeft, IEnumerable<TRight>?>> leftExpression,
        Expression<Func<TRight, IEnumerable<TLeft>?>> rightExpression, string leftName, string rightName) where TLeft : BaseObject where TRight : BaseObject
    {
        builder.Entity<TLeft>().HasMany(leftExpression).WithMany(rightExpression)
            .UsingEntity<Dictionary<string, object>>($"{leftName}_{rightName}_relation",
                j => j.HasOne<TRight>().WithMany().HasForeignKey(rightName).HasConstraintName($"fk_{leftName}_{rightName}"),
                j => j.HasOne<TLeft>().WithMany().HasForeignKey(leftName).HasConstraintName($"fk_{rightName}_{leftName}"),
                j =>
                {
                    j.Property<Guid>("id").HasColumnName("id");
                    j.Property<Guid>(leftName).HasColumnName(leftName);
                    j.Property<Guid>(rightName).HasColumnName(rightName);

                    j.HasKey("id").HasName($"pk_{leftName}_{rightName}_relation");
                    j.HasIndex([rightName], $"ix_{leftName}_{rightName}_relation_{rightName}");
                    j.HasIndex([leftName], $"ix_{leftName}_{rightName}_relation_{leftName}");
                });
    }
}