namespace Wallet.Modules.position_module
{
    public class PositionDTO
    {
        public string AssetName { get; set; }
        public string Ticker { get; set; }
        public double Amount { get; set; }
        public double AveragePrice { get; set; }
        public double Price { get; set; }
        public double Size { get; set; }
        public double RelativeSize { get; set; }
        public double TradeResult { get; set; }
        public double TradeResultPercentage { get; set; }
        public double TotalBought { get; set; }
        public double TotalSold { get; set; }
        public double Result { get; set; }
        public double ResultPercentage { get; set; }
    }
}
