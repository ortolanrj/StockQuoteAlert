using System.Net.Http.Json;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using static Google.Apis.Requests.BatchRequest;

namespace StockQuoteAlert.Services.StockAPI
{
    public class BrapiAPIService : IStockAPIService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly StockAPIOptions _stockAPIOptions;

        const string BrapiHttpClientName = "BrapiHttpClient";

        public BrapiAPIService(IHttpClientFactory httpClientFactory, 
                               IOptions<StockAPIOptions> stockAPIOptions)
        {
            _httpClientFactory = httpClientFactory;
            _stockAPIOptions = stockAPIOptions.Value;
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
                return brapiResponse?.Results.First().RegularMarketPrice;
                
            }
            catch (Exception ex)
            {
                throw new Exception($"Ocurred an error during the Stock API request.", ex);
            }
        }
    }
}
