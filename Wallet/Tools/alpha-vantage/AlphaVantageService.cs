using Newtonsoft.Json.Linq;
using System.Globalization;

namespace Wallet.Tools.alpha_vantage
{
    public class AlphaVantageService : IAlphaVantageService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AlphaVantageService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<decimal> GetStockQuote(string symbol)
        {
            string apiKey = _configuration["AlphaVantage:ApiKey"];

            // Construa a URL da API Alpha Vantage com base no símbolo do ativo e na sua API key
            string apiUrl = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={symbol}&apikey={apiKey}";

            // Faça a chamada à API Alpha Vantage
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            // Analise a resposta da API e extraia a cotação do ativo
            var responseData = await response.Content.ReadAsStringAsync();
            var quoteData = JObject.Parse(responseData)["Global Quote"];
            decimal stockQuote = decimal.Parse(quoteData["05. price"].ToString(), CultureInfo.InvariantCulture);

            return stockQuote;
        }

        // Implemente outros métodos para obter informações adicionais da API Alpha Vantage
    }
}
