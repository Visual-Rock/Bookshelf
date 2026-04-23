using Bookshelf.DataAccess;
using Bookshelf.DataModel;

namespace Bookshelf.Api.Services;

public interface ICategoryService
{
    Category GetOrCreate(string name);
}

public class CategoryService(BookshelfContext context) : ICategoryService
{
    public Category GetOrCreate(string name)
    {
        var category = context.Categories.FirstOrDefault(a => a.Name == name);

        if (category is null)
        {
            category = new Category
            {
                Id = Guid.CreateVersion7(),
                Name = name
            };

            context.Add(category);
            context.SaveChanges();
        }

        return category;
    }
}