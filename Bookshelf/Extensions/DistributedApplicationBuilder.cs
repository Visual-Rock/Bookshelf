using Projects;

namespace Bookshelf.Extensions;

public static class DistributedApplicationBuilder
{
    extension(IDistributedApplicationBuilder builder)
    {
        public IResourceBuilder<KeycloakResource> AddKeycloak()
        {
            var username = builder.AddParameter("keycloak-username", "admin");
            var password = builder.AddParameter("keycloak-password", "admin");
            var keycloak = builder.AddKeycloak("bookshelf-keycloak", 8080, username, password)
                .WithOtlpExporter().WithDataVolume()
                .WithLifetime(ContainerLifetime.Persistent);

            username.WithParentRelationship(keycloak);
            password.WithParentRelationship(keycloak);

            return keycloak;
        }

        public (IResourceBuilder<PostgresServerResource> server, IResourceBuilder<PostgresDatabaseResource> db,
            IResourceBuilder<ProjectResource>)
            AddBookshelfDatabase()
        {
            var username = builder.AddParameter("database-username", "postgres");
            var password = builder.AddParameter("database-password", "postgres");

            var postgres = builder.AddPostgres("bookshelf-database", username, password)
                .WithLifetime(ContainerLifetime.Persistent).WithDataVolume("bookshelf-db-data");
            username.WithParentRelationship(postgres);
            password.WithParentRelationship(postgres);

            var db = postgres.AddDatabase("bookshelf", "bookshelf");

            var migrationService = builder.AddProject<Bookshelf_MigrationService>("bookshelf-migration-service")
                .WaitFor(db)
                .WithReference(db);

            return (postgres, db, migrationService);
        }
    }
}