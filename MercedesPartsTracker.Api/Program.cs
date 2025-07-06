using MercedesPartsTracker.EntityFramework;
using MercedesPartsTracker.EntityFramework.Models;
using MercedesPartsTracker.EntityFrameworkServices.Implementations;
using MercedesPartsTracker.EntityFrameworkServices.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Npgsql;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => 
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Mercedes Parts Tracker API",
        Version = "v1",
        Description = "API for managing Mercedes car parts inventory."
    });
});

builder.Services.AddDbContext<DBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPartService, PartService>();

var app = builder.Build();

app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mercedes Parts Tracker API v1"); 
});

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DBContext>();

    dbContext.Database.Migrate();

    if (!dbContext.Parts.Any())
    {
        dbContext.Parts.AddRange(
            new Part
            {
                PartNumber = "P1001",
                Description = "Engine Oil Filter",
                QuantityOnHand = 25,
                LocationCode = "L01",
                LastStockTake = DateTime.UtcNow
            },
            new Part
            {
                PartNumber = "P1002",
                Description = "Brake Pad Set",
                QuantityOnHand = 40,
                LocationCode = "L02",
                LastStockTake = DateTime.UtcNow
            },
            new Part
            {
                PartNumber = "P1003",
                Description = "Air Conditioning Filter",
                QuantityOnHand = 15,
                LocationCode = "L03",
                LastStockTake = DateTime.UtcNow
            }
        );
        dbContext.SaveChanges();
    }
}

app.MapGet("/health", async (DBContext dbContext) =>
{
    var dbCanConnect = await dbContext.Database.CanConnectAsync();
    return Results.Json(new
    {
        Service = "Running",
        Database = dbCanConnect ? "Available" : "Unavailable"
    });
});

app.Run();