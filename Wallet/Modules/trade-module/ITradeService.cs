using System.Threading.Tasks;
using Wallet.Tools.generic_module;

namespace Wallet.Modules.trade_module
{
    internal interface ITradeService : IGenericService<Trade>
    {
        Task<Trade> Creat(Trade trade);
        Task<Trade> Delete(string id);
        Task<Trade> Read(string? id);
        Task<Trade> Update(string id, Trade trade);
    }
}