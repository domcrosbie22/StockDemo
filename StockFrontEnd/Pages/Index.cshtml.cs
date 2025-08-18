using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockFrontEnd.Models;
using System.Text.Json;

namespace StockFrontEnd.Pages;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public IList<Stock> Portfolio { get; set; } = new List<Stock>();
    public decimal TotalValue { get; set; }
    public Stock? HighestFluctuationStock { get; set; }

        public async Task OnGetAsync()
    {
        var client = _httpClientFactory.CreateClient();
        // The port number (e.g., 7132) should match the one your API is running on.
        // This might need to be adjusted based on your launchSettings.json in the StockApi project.
        var response = await client.GetAsync("http://localhost:5212/portfolio");

        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            Portfolio = JsonSerializer.Deserialize<List<Stock>>(jsonString, options) ?? new List<Stock>();

            TotalValue = Portfolio.Sum(s => s.Price);

            if (Portfolio.Any())
            {
                 HighestFluctuationStock = Portfolio.OrderByDescending(s => Math.Abs(s.Change)).First();
            }
        }
    }

    public async Task<IActionResult> OnPostAsync(int id, decimal price)
    {
        var client = _httpClientFactory.CreateClient();
        var priceUpdate = new PriceUpdate { Price = price };
        var jsonContent = new StringContent(JsonSerializer.Serialize(priceUpdate), System.Text.Encoding.UTF8, "application/json");

        // The port number (e.g., 7132) should match the one your API is running on.
        var response = await client.PostAsync($"http://localhost:5212/portfolio/{id}", jsonContent);

        if (!response.IsSuccessStatusCode)
        {
            // Handle error appropriately
            // For now, just logging and redirecting
            Console.WriteLine($"Error updating stock {id}: {response.ReasonPhrase}");
        }

        return RedirectToPage();
    }
}
