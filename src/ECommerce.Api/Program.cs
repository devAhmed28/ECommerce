using ECommerce.Api.Middleware;
using ECommerce.Modules.Catalog;

var builder = WebApplication.CreateBuilder(args);

// add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' was not found");

builder.Services.AddCatalogModule(connectionString);

var app = builder.Build();

// middleware pipeline
app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ECommerce API V1");
    });
}

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();