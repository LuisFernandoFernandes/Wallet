using Wallet.Tools.generic_module;

namespace Wallet.Modules.asset_module
{
    public interface IAssetService : IGenericService<Asset>
    {
        Task<Asset> Creat(Asset asset);
        Task<List<Asset>> Read(string? id, string? ticker);
        Task<Asset> Update(string id, Asset asset);
        Task<Asset> Delete(string id);
        Task SetStockQuote(List<Asset> assets);
    }
}