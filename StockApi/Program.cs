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

// OAuth2 token endpoint that matches the expected format for the C# client
app.MapPost("/openam/oauth2/realms/root/realms/Employees/access_token", async (HttpRequest request) =>
{
    try
    {
        // Ensure the content type is form-urlencoded
        if (!request.HasFormContentType)
        {
            return Results.BadRequest(new 
            {
                error = "invalid_request",
                error_description = "The request must be application/x-www-form-urlencoded"
            });
        }

        // Read the form data
        var form = await request.ReadFormAsync();
        
        // Get the form values
        var grantType = form["grant_type"].FirstOrDefault();
        var username = form["username"].FirstOrDefault();
        var password = form["password"].FirstOrDefault();
        var scope = form["scope"].FirstOrDefault()?.ToString().Trim();
        
        // Log the received values for debugging
        Console.WriteLine($"Received token request - grant_type: {grantType}, username: {username}, scope: {scope}");
        
        // Validate credentials (hardcoded for now)
        if (grantType == "password" && 
            username == "PKKT10" && 
            password == "Kransekak1" && 
            scope == "openid svvprofile")
    {
            // Return a proper OAuth2 token response
            var tokenResponse = new
            {
                access_token = "dummy_access_token_12345",
                token_type = "Bearer",
                expires_in = 3600,
                scope = "openid svvprofile",
                id_token = "dummy_id_token_12345"
            };
            
            Console.WriteLine("Authentication successful, returning token");
            return Results.Ok(tokenResponse);
    }
    
        // Return standard OAuth2 error response for invalid credentials
        Console.WriteLine("Authentication failed - invalid credentials");
        return Results.BadRequest(new 
        {
            error = "invalid_grant",
            error_description = "Invalid username or password"
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error processing token request: {ex}");
        return Results.Problem("An error occurred while processing your request", statusCode: 500);
    }
})
.WithName("Login")
.WithOpenApi();

app.Run();

public class PriceUpdate
{
    public decimal Price { get; set; }
}
