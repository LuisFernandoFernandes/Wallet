using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Net;
using Wallet.Modules.asset_module;
using Wallet.Tools.database;

namespace Wallet.Tools.alpha_vantage
{
    public class AlphaVantageService : IAlphaVantageService
    {
        private readonly Context _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AlphaVantageService(Context context, HttpClient httpClient, IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<double> GetStockQuote(string symbol)
        {
            string apiKey = _configuration["AlphaVantage:ApiKey"];

            string apiUrl = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={symbol}&apikey={apiKey}";

            // Faça a chamada à API Alpha Vantage
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            // Analise a resposta da API e extraia a cotação do ativo
            var responseData = await response.Content.ReadAsStringAsync();
            var quoteData = JObject.Parse(responseData)["Global Quote"];
            double stockQuote = double.Parse(quoteData["05. price"].ToString(), CultureInfo.InvariantCulture);

            return stockQuote;
        }

        // Implemente outros métodos para obter informações adicionais da API Alpha Vantage


        /// <summary>
        /// Return a list of tickers that correspond to a search.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public async Task<List<SearchResult>> SearchTicker(string symbol)
        {
            string apiKey = _configuration["AlphaVantage:ApiKey"];

            string queryUrl = $"https://www.alphavantage.co/query?function=SYMBOL_SEARCH&keywords={symbol}&apikey={apiKey}";

            HttpResponseMessage response = await _httpClient.GetAsync(queryUrl);
            response.EnsureSuccessStatusCode();

            var responseData = await response.Content.ReadAsStringAsync();
            var jsonData = JObject.Parse(responseData)["bestMatches"];

            List<SearchResult> searchResults = new List<SearchResult>();
            foreach (var item in jsonData)
            {
                SearchResult result = new SearchResult
                {
                    Symbol = item["1. symbol"].ToString(),
                    Name = item["2. name"].ToString(),
                    Type = item["3. type"].ToString(),
                    Region = item["4. region"].ToString(),
                    MarketOpen = item["5. marketOpen"].ToString(),
                    MarketClose = item["6. marketClose"].ToString(),
                    Timezone = item["7. timezone"].ToString(),
                    Currency = item["8. currency"].ToString(),
                    MatchScore = double.Parse(item["9. matchScore"].ToString(), CultureInfo.InvariantCulture)
                };

                searchResults.Add(result);
            }

            return searchResults;
        }

    }
}
