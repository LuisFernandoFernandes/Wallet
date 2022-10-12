using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Wallet.Modules.trade_module.enums
{
    public enum eTradeType
    {
        [Display(Name = "Buy")]
        Buy = 0,

        [Display(Name = "Sell")]
        Sell = 1,
    }
}
