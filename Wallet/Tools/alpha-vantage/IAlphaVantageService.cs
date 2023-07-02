using Wallet.Modules.asset_module;

namespace Wallet.Tools.alpha_vantage
{
    public interface IAlphaVantageService
    {
        Task ReloadStockQuotes();
        Task<double> GetStockQuote(string symbol);
    }
}
