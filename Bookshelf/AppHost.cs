using Aspire.Hosting.JavaScript;
using Aurora.AppHost.Extensions;
using Bookshelf.Extensions;
using Bookshelf.Helper;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var keycloak = builder.AddKeycloak();
var (_, db, dbMigrator) = builder.AddBookshelfDatabase();

var api = builder.AddProject<Bookshelf_Api>("bookshelf-api")
    .WithApiReference()
    .WaitForCompletion(dbMigrator).WithReference(db);

builder.AddViteApp("bookshelf-web", "../Bookshelf.Web")
    .WithReference(api)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

KeycloakHelper.ConfigureKeycloak(keycloak, api);
KeycloakHelper.AddKeycloakEnvironment(keycloak, api);

builder.Build().Run();