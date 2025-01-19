using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using StockQuoteAlert.Services.Email;

var builder = new ConfigurationBuilder();

builder.SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Configuring basic logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Build())
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        services.AddSingleton<IEmailService, EmailService>();

        // Configuring an instance of a HttpClient for the Brapi Stock API
        services.AddHttpClient("BrapiHttpClient", httpClient =>
        {
            httpClient.BaseAddress = new Uri(configuration.GetValue<string>("BrapiApi:BaseURL"));
            httpClient.DefaultRequestHeaders.Add("User-Agent", "StockQuoteAlert");
        });
    })
    .UseSerilog()
    .Build();

var svc = ActivatorUtilities.CreateInstance<EmailService>(host.Services);
svc.SendEmail();
