
namespace StockQuoteAlert.Services.StockAPI
{
    public interface IStockAPIService
    {
        Task<decimal?> GetStockPriceAsync(string ticker);
    }
}