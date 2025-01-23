using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace StockQuoteAlert.Services.StockAPI
{
    public class BrapiAPIService : IStockAPIService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly StockAPIOptions _stockAPIOptions;

        private readonly ILogger<BrapiAPIService> _log;

        private const string BrapiHttpClientName = "BrapiHttpClient";

        private const string MessageErrorStockAPIRequest = "Ocurred an error during the Stock API request.";

        private const string MessageSuccessStockAPIRequest = "The request to the Stock API was successful.";

        private const string MessageConsoleWrongBrapiAPIKey = "\nO token fornecido para a Brapi API é inválido. Por favor, configure um token válido no arquivo de configurações.";

        private const string MessageGenericErrorAPI = "\nErro ao enviar requisição para a API. Revise suas configurações, e pressione qualquer tecla para sair.";

        public BrapiAPIService(IHttpClientFactory httpClientFactory, 
                               IOptions<StockAPIOptions> stockAPIOptions,
                               ILogger<BrapiAPIService> log)
        {
            _httpClientFactory = httpClientFactory;
            _stockAPIOptions = stockAPIOptions.Value;
            _log = log;
        }

        public async Task<decimal?> GetStockPriceAsync(string ticker) 
        {
            var client = _httpClientFactory.CreateClient(BrapiHttpClientName);
            var token = _stockAPIOptions.Key;

            try
            {
                var httpResponse = await client.GetAsync($"{ticker}?token={token}");

                httpResponse.EnsureSuccessStatusCode();

                var stringData = await httpResponse.Content.ReadAsStringAsync();
                var brapiResponse = JsonConvert.DeserializeObject<BrapiAPIResponse>(stringData);

                _log.LogInformation(MessageSuccessStockAPIRequest);
                return brapiResponse?.Results.First().RegularMarketPrice;
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode.Equals(HttpStatusCode.NotFound))
                {
                    _log.LogError(ex, $"{MessageErrorStockAPIRequest}. The ticker {ticker} was not found. {ex.Message}.");
                    Console.WriteLine($"\nAtivo {ticker} não encontrado! Por favor, digite um ticker válido.\nPressione qualquer tecla para sair.");
                }
                else if (ex.StatusCode.Equals(HttpStatusCode.Unauthorized))
                {
                    _log.LogError(ex, $"{MessageErrorStockAPIRequest}. The token for Brapi API is invalid. {ex.Message}.");
                    Console.WriteLine(MessageConsoleWrongBrapiAPIKey);
                }
                else
                {
                    _log.LogError(ex, $"{MessageErrorStockAPIRequest} {ex.Message}");
                    Console.WriteLine(MessageGenericErrorAPI);
                }
                return null;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"{MessageErrorStockAPIRequest} {ex.Message}");
                Console.WriteLine(MessageGenericErrorAPI);
                return null;
            }
        }
    }
}
