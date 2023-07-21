using System.Threading.Tasks;
using Wallet.Tools.generic_module;

namespace Wallet.Modules.trade_module
{
    internal interface ITradeService : IGenericService<Trade>
    {
        Task<List<Trade>> CreatList(List<TradeDTO> trades);
        Task<Trade> Creat(TradeDTO trade);
        Task Delete(string id);
        Task<List<Trade>> Read(string? id);
        Task<Trade> Update(Trade trade);
    }
}