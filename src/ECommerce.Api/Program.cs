using ECommerce.Api.Infrastructure;
using ECommerce.Api.Middleware;
using ECommerce.Modules.Cart;
using ECommerce.Modules.Catalog;
using ECommerce.Modules.Identity;
using ECommerce.Modules.Orders;
using ECommerce.Shared.Abstractions;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// add services


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("auth", limiterOptions =>
    {
        limiterOptions.PermitLimit = 10;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' was not found");

builder.Services.AddCatalogModule(connectionString);
builder.Services.AddIdentityModule(builder.Configuration, connectionString);
builder.Services.AddCartModule(connectionString);
builder.Services.AddOrdersModule(connectionString);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

var app = builder.Build();

// middleware pipeline

app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

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