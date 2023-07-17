using Wallet.Modules.asset_module;

namespace Wallet.Tools.alpha_vantage
{
    public interface IAlphaVantageService
    {
        Task<double> GetStockQuote(string symbol);
        Task<List<SearchResult>> SearchTicker(string symbol);
        Task<List<GetAllUSAssetsDTO>> GetAllUSAssets();
    }
}
