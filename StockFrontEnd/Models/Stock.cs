namespace StockFrontEnd.Models
{
    public class Stock
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public decimal Change { get; set; }
    }
}
