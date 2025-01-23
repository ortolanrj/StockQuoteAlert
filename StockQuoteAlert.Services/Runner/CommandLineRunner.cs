using System.Globalization;
using StockQuoteAlert.Services.Email;
using StockQuoteAlert.Services.StockAPI;

namespace StockQuoteAlert.Services.Runner;
public class CommandLineRunner : IRunner
{
    private readonly IStockAPIService _stockAPIService;

    private readonly IEmailService _emailService;

    private const int TimeInterval = 30;

    private const string MessageProblemFinanceInstrumentSearch = "Houve um problema na busca do preço do ativo.";
    
    private const string MessageProblemCmdArguments = "Por favor, utilize os parâmetros no seguinte formato:\n " +
                                                       "> .\\stock-quote-alert.exe <TICKER> <PRECO-VENDA> <PRECO-COMPRA>. Ex.: PETR4 22.67 22.59";

    private const string MessagePriceNotAboveOrUnderLimits = "Não houve variação do preço acima ou abaixo dos limites estabelecidos.";

    private const string PressKeyToEndProgramExecution = "Pressione qualquer tecla para encerrar o programa.";
 
    private decimal? price;

    public CommandLineRunner(IStockAPIService stockAPIService, IEmailService emailService)
    {
        _stockAPIService = stockAPIService;
        _emailService = emailService;
    }

    public void Run()
    {
        string[] args = Environment.GetCommandLineArgs();

        if (args.Length <= 1)
        {
            Console.WriteLine(MessageProblemCmdArguments);
            return;
        }

        var ticker = args[1];
        var sellPriceSuccess = Decimal.TryParse(args[2], new CultureInfo("en-US"), out var sellPrice);
        var buyPriceSuccess = Decimal.TryParse(args[3], new CultureInfo("en-US"), out var buyPrice);

        if (!sellPriceSuccess || !buyPriceSuccess)
        {
            Console.WriteLine(MessageProblemCmdArguments);
            return;
        }

        Stock stock = new(ticker, sellPrice, buyPrice);

        RunFromTimeToTime(stock);
    }

    private void RunFromTimeToTime(Stock stock)
    {
        Timer timer = new Timer(state => CheckPriceAndSendEmail(stock), null, TimeSpan.Zero, TimeSpan.FromMinutes(TimeInterval));

        // Gives the user an option to close the program
        Console.WriteLine($"Checaremos o preço da ação de {TimeInterval} em {TimeInterval} minutos.");
        Console.WriteLine(PressKeyToEndProgramExecution);
        Console.ReadKey(true);
    }

    private void CheckPriceAndSendEmail(Stock stock)
    {
        price = _stockAPIService.GetStockPriceAsync(stock.Ticker).Result;

        if (!price.HasValue)
        {
            Console.WriteLine(MessageProblemFinanceInstrumentSearch);
            return;
        }

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
            Console.WriteLine(MessagePriceNotAboveOrUnderLimits);
        }
    }
}
