using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using StockQuoteAlert.Services.Email;
using StockQuoteAlert.Services.Runner;
using StockQuoteAlert.Services.StockAPI;

var builder = new ConfigurationBuilder();

builder.SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
       .AddJsonFile("appsettings.user.json", optional: true, reloadOnChange: true);  

var configuration = builder.Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        // Configuring an instance of a HttpClient for the Brapi Stock API
        services.AddHttpClient("BrapiHttpClient", httpClient =>
        {
            httpClient.BaseAddress = new Uri(configuration.GetValue<string>("BrapiApi:BaseURL"));
            httpClient.DefaultRequestHeaders.Add("User-Agent", "StockQuoteAlert");
        });

        services.AddSingleton<IEmailService, SmtpService>();
        services.AddSingleton<IStockAPIService, BrapiAPIService>();
        services.AddSingleton<IConfiguration>(configuration);

        services.Configure<SmtpOptions>(configuration.GetSection("Smtp"));
        services.Configure<EmailOptions>(configuration.GetSection("Email"));
    })
    .UseSerilog()
    .Build();

var svc = ActivatorUtilities.CreateInstance<CommandLineRunner>(host.Services);
svc.Run();
