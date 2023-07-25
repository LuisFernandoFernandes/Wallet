using Wallet.Tools.alpha_vantage;
using Wallet.Tools.generic_module;

namespace Wallet.Modules.asset_module
{
    public interface IAssetHistoricalDataService : IGenericService<AssetHistoricalData>
    {
        Task AddHistoricalDataAsync(List<HistoricalDataDTO> assetHistoricalDataDTOList, string assetId);
    }
}
