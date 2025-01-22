﻿using StockQuoteAlert.Services.Email;
using StockQuoteAlert.Services.StockAPI;

namespace StockQuoteAlert.Services.Runner;
public class CommandLineRunner : IRunner
{
    private readonly IStockAPIService _stockAPIService;

    private readonly IEmailService _emailService;

    private const int TimeInterval = 30;

    public CommandLineRunner(IStockAPIService stockAPIService, IEmailService emailService)
    {
        _stockAPIService = stockAPIService;
        _emailService = emailService;
    }

    public void Run()
    {
        string[] args = Environment.GetCommandLineArgs();

        var ticker = args[1];
        var sellPrice = args[2];
        var buyPrice = args[3];

        Stock stock = new(ticker, decimal.Parse(sellPrice), decimal.Parse(buyPrice));

        RunFromTimeToTime(stock);
    }

    private void RunFromTimeToTime(Stock stock)
    {
        Timer timer = new Timer(state => CheckPriceAndSendEmail(stock), null, TimeSpan.Zero, TimeSpan.FromMinutes(TimeInterval));

        // Gives the user an option to close the program
        Console.WriteLine("Pressione qualquer tecla para encerrar o programa.");
        Console.ReadKey(true);
    }

    private void CheckPriceAndSendEmail(Stock stock)
    {
        decimal? price = _stockAPIService.GetStockPriceAsync(stock.Ticker).Result;

        if (price.HasValue) {
            stock.ActualPrice = price.Value;

            if (stock.ActualPrice > stock.SellPrice)
            {
                _emailService.SendEmail(true, stock);
            }
            else if (stock.ActualPrice < stock.BuyPrice)
            {
                _emailService.SendEmail(false, stock);
            }
            else
            {
                Console.WriteLine("Não houve variação do preço acima ou abaixo dos limites estabelecidos.");
            }
        } else
        {
            throw new Exception("Ocurred an error in the API call response.");
        }
    }
}
