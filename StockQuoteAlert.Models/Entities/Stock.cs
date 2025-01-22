public class Stock
{
    public decimal MaxPrice { get; set; }
    
    public decimal MinPrice { get; set; }
    
    public string Ticker { get; set; }

    public decimal ActualPrice { get; set; }

    public Stock(string ticker, decimal maxPrice, decimal minPrice)
    {
       Ticker = ticker;
       MaxPrice = maxPrice;
       MinPrice = minPrice;
    }
}