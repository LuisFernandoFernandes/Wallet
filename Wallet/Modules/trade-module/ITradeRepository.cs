namespace Wallet.Modules.trade_module
{
    public interface ITradeRepository
    {
        Task<IEnumerable<Trade>> List();
        Task<IEnumerable<Trade>> List(string id);
    }
}