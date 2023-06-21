using System.Threading.Tasks;
using Wallet.Tools.generic_module;

namespace Wallet.Modules.trade_module
{
    internal interface ITradeService : IGenericService<Trade>
    {
        Task<Trade> Creat(Trade trade);
        Task Delete(string id);
        Task<List<Trade>> Read(string? id);
        Task<Trade> Update(Trade trade);
    }
}