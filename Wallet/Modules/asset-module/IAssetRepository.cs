namespace Wallet.Modules.asset_module
{
    public interface IAssetRepository
    {
        Task<IEnumerable<Asset>> List();
        Task<IEnumerable<Asset>> List(string id);

    }
}