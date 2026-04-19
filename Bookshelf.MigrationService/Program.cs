using Bookshelf.DataAccess;
using Bookshelf.MigrationService;

var builder = Host.CreateApplicationBuilder(args);
builder.AddNpgsqlDbContext<BookshelfContext>("bookshelf");
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();