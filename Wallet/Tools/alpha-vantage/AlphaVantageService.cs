using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Globalization;
using Wallet.Modules.asset_module;
using Wallet.Tools.database;

namespace Wallet.Tools.alpha_vantage
{
    public class AlphaVantageService : IAlphaVantageService
    {
        private readonly Context _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IAssetService _assetService;

        public AlphaVantageService(Context context, HttpClient httpClient, IConfiguration configuration, IAssetService assetService)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;
            _assetService = assetService;
        }

        public async Task ReloadStockQuotes()
        {
            var assets = await _context.Asset.AsQueryable().Join(_context.Position, asset => asset.Id, position => position.AssetId, (asset, position) => asset).Distinct().ToListAsync();
            foreach (var asset in assets)
            {
                try
                {
                    var stockQuote = await GetStockQuote(asset.Ticker);
                    asset.Price = stockQuote;
                }
                catch (Exception)
                {

                    continue;
                }

            }

            await _assetService.SetStockQuote(assets);
        }


        public async Task<double> GetStockQuote(string symbol)
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
            double stockQuote = double.Parse(quoteData["05. price"].ToString(), CultureInfo.InvariantCulture);

            return stockQuote;
        }

        // Implemente outros métodos para obter informações adicionais da API Alpha Vantage
    }
}
