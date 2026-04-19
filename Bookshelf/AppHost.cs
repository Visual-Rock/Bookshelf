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

var web = builder.AddViteApp("bookshelf-web", "../Bookshelf.Web")
    .WithReference(api).WithEndpoint("http", annotation => annotation.Port = 5028);

KeycloakHelper.ConfigureKeycloak(keycloak, api, web);
KeycloakHelper.AddKeycloakEnvironment(keycloak, api);

api.WithEnvironment("Cors:AllowedOrigins", web.GetEndpoint().EndpointAnnotation.GetUrl());

builder.Build().Run();