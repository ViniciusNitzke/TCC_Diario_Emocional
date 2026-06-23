using DiarioEmocional.Api.Api;
using DiarioEmocional.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDiarioEmocional();

var app = builder.Build();

app.UseCors();
app.MapApiEndpoints();

app.Run();
