using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Bookshelf_Api>("bookshelf-api");

builder.Build().Run();