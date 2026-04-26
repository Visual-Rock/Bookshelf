using Bookshelf.DataAccess;
using Bookshelf.DataModel;

namespace Bookshelf.Api.Services;

public interface IPublisherService
{
    Publisher GetOrCreate(string name);
}

public class PublisherService(BookshelfContext context) : IPublisherService
{
    public Publisher GetOrCreate(string name)
    {
        var publisher = context.Publishers.FirstOrDefault(a => a.Name == name);

        if (publisher is null)
        {
            publisher = new Publisher
            {
                Id = Guid.CreateVersion7(),
                Name = name
            };

            context.Add(publisher);
            context.SaveChanges();
        }

        return publisher;
    }
}
