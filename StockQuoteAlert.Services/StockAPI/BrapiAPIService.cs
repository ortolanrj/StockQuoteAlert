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

        const string BrapiHttpClientName = "BrapiHttpClient";

        const string MessageErrorStockAPIRequest = "Ocurred an error during the Stock API request.";

        const string MessageSuccessStockAPIRequest = "The request to the Stock API was successful.";

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
            catch (Exception ex)
            {
                _log.LogError($"{MessageErrorStockAPIRequest} {ex.Message}");
                throw new Exception(MessageErrorStockAPIRequest, ex);
            }
        }
    }
}
