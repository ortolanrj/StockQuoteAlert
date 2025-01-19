
namespace StockQuoteAlert.Services.StockAPI
{
    public interface IStockAPIService
    {
        Task GetStockPrice(string ticker);
    }
}