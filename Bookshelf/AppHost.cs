using Aurora.AppHost.Extensions;
using Bookshelf.Extensions;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var (_, db, dbMigrator) = builder.AddBookshelfDatabase();

var api = builder.AddProject<Bookshelf_Api>("bookshelf-api")
    .WithApiReference()
    .WaitForCompletion(dbMigrator).WithReference(db);

builder.Build().Run();