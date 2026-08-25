using ECommerce.Api.Middleware;
using ECommerce.Modules.Catalog;

var builder = WebApplication.CreateBuilder(args);

// add services
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

builder.Services.AddCatalogModule();


var app = builder.Build();

// middleware pipeline
app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "ECommerce API v1");
    });
}


app.MapHealthChecks("/health");

app.Run();