namespace Wallet.Tools.alpha_vantage
{
    public interface IAlphaVantageService
    {
        Task<decimal> GetStockQuote(string symbol);
    }
}
