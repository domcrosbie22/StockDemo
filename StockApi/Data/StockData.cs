using StockApi.Models;
using System.Collections.Generic;

namespace StockApi.Data
{
    public static class StockData
    {
        public static List<Stock> Stocks { get; } = new List<Stock>
        {
            new Stock { Id = 1, Name = "Microsoft", Price = 300.00m, Change = 0.0m },
            new Stock { Id = 2, Name = "Apple", Price = 150.00m, Change = 0.0m },
            new Stock { Id = 3, Name = "Google", Price = 2800.00m, Change = 0.0m },
            new Stock { Id = 4, Name = "Amazon", Price = 3400.00m, Change = 0.0m },
            new Stock { Id = 5, Name = "Tesla", Price = 750.00m, Change = 0.0m }
        };
    }
}
