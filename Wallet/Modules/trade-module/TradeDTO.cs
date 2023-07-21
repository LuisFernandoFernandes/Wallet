using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wallet.Modules.trade_module.enums;
using Wallet.Modules.user_module;

namespace Wallet.Modules.trade_module
{
    public class TradeDTO
    {

        public eTradeType Type { get; set; }

        public string Date { get; set; }

        public string Ticker { get; set; }

        public double Amount { get; set; }

        public double Price { get; set; }
    }
}
