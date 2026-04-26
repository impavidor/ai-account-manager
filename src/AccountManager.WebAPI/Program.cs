using AccountManager.Application;
using AccountManager.Infrastructure;
using AccountManager.WebAPI;

var builder = WebApplication.CreateBuilder(args);

var dataPath = builder.Configuration["DataPath"]
    ?? Path.Combine(builder.Environment.ContentRootPath, "data");

builder.Services.AddControllers();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(dataPath);
builder.Services.AddWebApiServices();

var app = builder.Build();

app.UseMiddleware<FakeAuthMiddleware>();
app.MapControllers();

app.Run();

public partial class Program { }
