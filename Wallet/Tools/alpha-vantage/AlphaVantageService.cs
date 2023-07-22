using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using Wallet.Modules.asset_module;
using Wallet.Tools.database;

namespace Wallet.Tools.alpha_vantage
{
    public class AlphaVantageService : IAlphaVantageService
    {
        private const int RequestsPerMinuteLimit = 5;
        private const int RequestsPerDayLimit = 500;
        private const int RateLimitResetMinutes = 60;

        private readonly Context _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private string _apiKey; // Propriedade para armazenar a chave da API
        private DateTime _lastRequestTime;
        private int _requestsCountMinute;
        private int _requestsCountDay;

        public AlphaVantageService(Context context, HttpClient httpClient, IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;

            _apiKey = _configuration["AlphaVantage:ApiKey"]; // Obter a chave da API do arquivo de configuração
            _lastRequestTime = DateTime.MinValue;
            _requestsCountMinute = 0;
            _requestsCountDay = 0;
        }

        private async Task WaitForRateLimit()
        {
            // Verifique se já atingiu o limite de requisições por minuto
            if (_requestsCountMinute >= RequestsPerMinuteLimit)
            {
                var elapsedTime = DateTime.UtcNow - _lastRequestTime;
                var timeToWait = TimeSpan.FromSeconds(RateLimitResetMinutes) - elapsedTime;

                // Espere pelo tempo restante até o próximo intervalo de minutos
                if (timeToWait > TimeSpan.Zero)
                {
                    await Task.Delay(timeToWait);
                }
            }

            // Verifique se já atingiu o limite de requisições por dia
            if (_requestsCountDay >= RequestsPerDayLimit)
            {
                throw new Exception("Limite diário de requisições atingido.");
            }
        }

        private void UpdateRequestsCount()
        {
            var currentTime = DateTime.UtcNow;

            // Se já passou para o próximo minuto, reinicie o contador de requisições por minuto
            if (currentTime.Subtract(_lastRequestTime).TotalMinutes >= 1)
            {
                _requestsCountMinute = 1;
            }
            else
            {
                _requestsCountMinute++;
            }

            // Se já passou para o próximo dia, reinicie o contador de requisições por dia
            if (currentTime.Date > _lastRequestTime.Date)
            {
                _requestsCountDay = 1;
            }
            else
            {
                _requestsCountDay++;
            }

            _lastRequestTime = currentTime;
        }

        public async Task<double> GetStockQuote(string symbol)
        {
            await WaitForRateLimit();

            string apiUrl = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={symbol}&apikey={_apiKey}";

            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            UpdateRequestsCount();
            response.EnsureSuccessStatusCode();

            var responseData = await response.Content.ReadAsStringAsync();
            var quoteData = JObject.Parse(responseData)["Global Quote"];
            double stockQuote = double.Parse(quoteData["05. price"].ToString(), CultureInfo.InvariantCulture);

            return stockQuote;
        }

        /// <summary>
        /// Return a list of tickers that correspond to a search.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public async Task<List<SearchResult>> SearchTicker(string symbol)
        {
            await WaitForRateLimit();

            string queryUrl = $"https://www.alphavantage.co/query?function=SYMBOL_SEARCH&keywords={symbol}&apikey={_apiKey}";

            HttpResponseMessage response = await _httpClient.GetAsync(queryUrl);
            UpdateRequestsCount();

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

        public async Task<List<GetAllUSAssetsDTO>> GetAllUSAssets()
        {
            try
            {
                await WaitForRateLimit();

                var symbols = new List<GetAllUSAssetsDTO>();

                var url = $"https://www.alphavantage.co/query?function=LISTING_STATUS&apikey={_apiKey}";

                var response = await _httpClient.GetAsync(url);
                UpdateRequestsCount();

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();

                    // Ler o arquivo CSV usando a biblioteca CsvHelper
                    using (var reader = new StringReader(data))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        var records = csv.GetRecords<GetAllUSAssetsDTO>();
                        symbols.AddRange(records);
                    }
                }
                else
                {
                    throw new Exception($"Erro ao obter a lista de símbolos: {response.StatusCode}");
                }

                return symbols;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<CompanyOverviewDTO>> GetCompanyOverview(List<string> symbols)
        {
            await WaitForRateLimit();

            List<CompanyOverviewDTO> companyOverviews = new List<CompanyOverviewDTO>();

            foreach (var symbol in symbols)
            {
                // Construa a URL da solicitação para obter informações sobre a empresa associada ao ativo
                var url = $"https://www.alphavantage.co/query?function=OVERVIEW&symbol={symbol}&apikey={_apiKey}";

                // Envie a solicitação GET para a API do Alpha Vantage
                var response = await _httpClient.GetAsync(url);
                UpdateRequestsCount();

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(data);

                    // Crie um objeto CompanyOverviewDTO e preencha as propriedades com os dados do overview
                    var overviewDTO = new CompanyOverviewDTO
                    {
                        Symbol = json["Symbol"]?.ToString(),
                        AssetType = json["AssetType"]?.ToString(),
                        Name = json["Name"]?.ToString(),
                        Description = json["Description"]?.ToString(),
                        CIK = json["CIK"]?.ToString(),
                        Exchange = json["Exchange"]?.ToString(),
                        Currency = json["Currency"]?.ToString(),
                        Country = json["Country"]?.ToString(),
                        Sector = json["Sector"]?.ToString(),
                        Industry = json["Industry"]?.ToString(),
                        Address = json["Address"]?.ToString(),
                        FiscalYearEnd = json["FiscalYearEnd"]?.ToString(),
                        LatestQuarter = DateTime.Parse(json["LatestQuarter"]?.ToString()),
                        MarketCapitalization = long.Parse(json["MarketCapitalization"]?.ToString()),
                        EBITDA = long.Parse(json["EBITDA"]?.ToString()),
                        PERatio = double.Parse(json["PERatio"]?.ToString()),
                        PEGRatio = double.Parse(json["PEGRatio"]?.ToString()),
                        BookValue = double.Parse(json["BookValue"]?.ToString()),
                        DividendPerShare = double.Parse(json["DividendPerShare"]?.ToString()),
                        DividendYield = double.Parse(json["DividendYield"]?.ToString()),
                        EPS = double.Parse(json["EPS"]?.ToString()),
                        RevenuePerShareTTM = double.Parse(json["RevenuePerShareTTM"]?.ToString()),
                        ProfitMargin = double.Parse(json["ProfitMargin"]?.ToString()),
                        OperatingMarginTTM = double.Parse(json["OperatingMarginTTM"]?.ToString()),
                        ReturnOnAssetsTTM = double.Parse(json["ReturnOnAssetsTTM"]?.ToString()),
                        ReturnOnEquityTTM = double.Parse(json["ReturnOnEquityTTM"]?.ToString()),
                        RevenueTTM = long.Parse(json["RevenueTTM"]?.ToString()),
                        GrossProfitTTM = long.Parse(json["GrossProfitTTM"]?.ToString()),
                        DilutedEPSTTM = double.Parse(json["DilutedEPSTTM"]?.ToString()),
                        QuarterlyEarningsGrowthYOY = double.Parse(json["QuarterlyEarningsGrowthYOY"]?.ToString()),
                        QuarterlyRevenueGrowthYOY = double.Parse(json["QuarterlyRevenueGrowthYOY"]?.ToString()),
                        AnalystTargetPrice = double.Parse(json["AnalystTargetPrice"]?.ToString()),
                        TrailingPE = double.Parse(json["TrailingPE"]?.ToString()),
                        ForwardPE = double.Parse(json["ForwardPE"]?.ToString()),
                        PriceToSalesRatioTTM = double.Parse(json["PriceToSalesRatioTTM"]?.ToString()),
                        PriceToBookRatio = double.Parse(json["PriceToBookRatio"]?.ToString()),
                        EVToRevenue = double.Parse(json["EVToRevenue"]?.ToString()),
                        EVToEBITDA = double.Parse(json["EVToEBITDA"]?.ToString()),
                        Beta = double.Parse(json["Beta"]?.ToString()),
                        Week52High = double.Parse(json["52WeekHigh"]?.ToString()),
                        Week52Low = double.Parse(json["52WeekLow"]?.ToString()),
                        MovingAverage50Day = double.Parse(json["50DayMovingAverage"]?.ToString()),
                        MovingAverage200Day = double.Parse(json["200DayMovingAverage"]?.ToString()),
                        SharesOutstanding = long.Parse(json["SharesOutstanding"]?.ToString()),
                        DividendDate = DateTime.Parse(json["DividendDate"]?.ToString()),
                        ExDividendDate = DateTime.Parse(json["ExDividendDate"]?.ToString())
                    };

                    companyOverviews.Add(overviewDTO);
                }
                else
                {
                    continue;
                }
            }
            return companyOverviews;
        }
    }
}
