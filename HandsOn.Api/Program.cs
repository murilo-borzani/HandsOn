using HandsOn.Api;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.RegistraServicos(builder.Configuration);

var app = builder.Build();

app.ConfiguraApp();