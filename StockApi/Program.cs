using Microsoft.AspNetCore.Mvc;
using StockApi.Data;
using StockApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS policy
app.UseCors("AllowAll");

app.MapGet("/portfolio", () =>
{
    return Results.Ok(StockData.Stocks);
})
.WithName("GetPortfolio")
.WithOpenApi();

app.MapPost("/portfolio/{id}", (int id, [FromBody] PriceUpdate priceUpdate) =>
{
    var stock = StockData.Stocks.FirstOrDefault(s => s.Id == id);
    if (stock == null)
    {
        return Results.NotFound();
    }

    var oldPrice = stock.Price;
    stock.Price = priceUpdate.Price;
    stock.Change = stock.Price - oldPrice;

    return Results.Ok(stock);
})
.WithName("UpdateStockPrice")
.WithOpenApi();

app.Run();

public class PriceUpdate
{
    public decimal Price { get; set; }
}
