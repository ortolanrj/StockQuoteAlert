using Microsoft.Extensions.Logging;
using StockQuoteAlert.Services.StockAPI;

namespace StockQuoteAlert.Services.Email;
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _log;

    private readonly IStockAPIService _stockAPIService;

    public EmailService(ILogger<EmailService> log, IStockAPIService stockAPIService)
    {
        _log = log;
        _stockAPIService = stockAPIService;
    }

    public void SendEmail()
    {
        // TODO: Implement method that verifies every 30 minutes if the price goes beyond the limits

        var stockPrice = _stockAPIService.GetStockPriceAsync("PETR4").Result;
        Console.WriteLine($"The stock price is: {stockPrice.ToString()}");
        Console.ReadKey(true);
    }
}

