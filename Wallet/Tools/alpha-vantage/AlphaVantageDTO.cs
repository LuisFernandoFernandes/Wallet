using CsvHelper.Configuration;

namespace Wallet.Tools.alpha_vantage
{
    public class SearchResult
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Region { get; set; }
        public string MarketOpen { get; set; }
        public string MarketClose { get; set; }
        public string Timezone { get; set; }
        public string Currency { get; set; }
        public double MatchScore { get; set; }
    }

    public class GetAllUSAssetsDTO
    {
        public string symbol { get; set; }
        public string name { get; set; }
        public string exchange { get; set; }
        public string assetType { get; set; }
        public DateTime ipoDate { get; set; }
        public string delistingDate { get; set; }
        public string status { get; set; }
    }

    public class CompanyOverviewDTO
    {
        public string Symbol { get; set; }
        public string AssetType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CIK { get; set; }
        public string Exchange { get; set; }
        public string Currency { get; set; }
        public string Country { get; set; }
        public string Sector { get; set; }
        public string Industry { get; set; }
        public string Address { get; set; }
        public string FiscalYearEnd { get; set; }
        public DateTime LatestQuarter { get; set; }
        public long MarketCapitalization { get; set; }
        public long EBITDA { get; set; }
        public double PERatio { get; set; }
        public double PEGRatio { get; set; }
        public double BookValue { get; set; }
        public double DividendPerShare { get; set; }
        public double DividendYield { get; set; }
        public double EPS { get; set; }
        public double RevenuePerShareTTM { get; set; }
        public double ProfitMargin { get; set; }
        public double OperatingMarginTTM { get; set; }
        public double ReturnOnAssetsTTM { get; set; }
        public double ReturnOnEquityTTM { get; set; }
        public long RevenueTTM { get; set; }
        public long GrossProfitTTM { get; set; }
        public double DilutedEPSTTM { get; set; }
        public double QuarterlyEarningsGrowthYOY { get; set; }
        public double QuarterlyRevenueGrowthYOY { get; set; }
        public double AnalystTargetPrice { get; set; }
        public double TrailingPE { get; set; }
        public double ForwardPE { get; set; }
        public double PriceToSalesRatioTTM { get; set; }
        public double PriceToBookRatio { get; set; }
        public double EVToRevenue { get; set; }
        public double EVToEBITDA { get; set; }
        public double Beta { get; set; }
        public double Week52High { get; set; }
        public double Week52Low { get; set; }
        public double MovingAverage50Day { get; set; }
        public double MovingAverage200Day { get; set; }
        public long SharesOutstanding { get; set; }
        public DateTime DividendDate { get; set; }
        public DateTime ExDividendDate { get; set; }
    }

    public class GetMyAssetsDTO
    {
        public string AV { get; set; }
        public string EMPRESA { get; set; }
        public string LISTA { get; set; }
    }

    public class HistoricalDataDTO
    {
        public DateTime Date { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double AdjustedClose { get; set; }
        public long Volume { get; set; }
        public double DividendAmount { get; set; }
        public double SplitCoefficient { get; set; }
    }
}
