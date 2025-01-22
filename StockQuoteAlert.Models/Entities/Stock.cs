public class Stock
{
    public decimal SellPrice { get; set; }
    
    public decimal BuyPrice { get; set; }
    
    public string Ticker { get; set; }

    public decimal ActualPrice { get; set; }

    public Stock(string ticker, decimal sellPrice, decimal buyPrice)
    {
       Ticker = ticker;
       SellPrice = sellPrice;
       BuyPrice = buyPrice;
    }
}