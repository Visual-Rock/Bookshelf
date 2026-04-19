namespace Bookshelf.Extensions;

public static class DistributedApplicationBuilder
{
    public static (IResourceBuilder<PostgresServerResource> server, IResourceBuilder<PostgresDatabaseResource> db)
        AddBookshelfDatabase(this IDistributedApplicationBuilder builder)
    {
        var username = builder.AddParameter("database-username", "postgres");
        var password = builder.AddParameter("database-password", "postgres");

        var postgres = builder.AddPostgres("bookshelf-database", username, password)
            .WithLifetime(ContainerLifetime.Persistent).WithDataVolume("bookshelf-db-data");
        username.WithParentRelationship(postgres);
        password.WithParentRelationship(postgres);

        var db = postgres.AddDatabase("bookshelf", "bookshelf");
        
        return (postgres, db);
    }
}