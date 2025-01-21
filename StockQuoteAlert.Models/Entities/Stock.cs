public class Stock
{
    public string Ticker { get; set; }

    public decimal MaxPrice { get; set; }

    public decimal MinPrice { get; set; }

    public Stock(string ticker, decimal maxPrice, decimal minPrice)
    {
        Ticker = ticker;
        MaxPrice = maxPrice;
        MinPrice = minPrice;
    }

}
