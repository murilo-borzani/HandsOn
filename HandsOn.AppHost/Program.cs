var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.HandsOn_Api>("handson-api");

builder.Build().Run();
