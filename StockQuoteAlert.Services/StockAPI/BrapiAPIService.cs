using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace StockQuoteAlert.Services.StockAPI
{
    public class BrapiAPIService : IStockAPIService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly IConfiguration _config;

        public BrapiAPIService(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public async Task<decimal> GetStockPriceAsync(string ticker)
        {
            var client = _httpClientFactory.CreateClient("BrapiHttpClient");
            var token = _config.GetValue<string>("BrapiApi:Key");

            var response = await client.GetFromJsonAsync<BrapiAPIResponse>($"{ticker}?token={token}");

            return response.Results.First().RegularMarketPrice;
        }
    }
}
