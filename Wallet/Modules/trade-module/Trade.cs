using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;
using Wallet.Modules.asset_module;
using Wallet.Modules.trade_module.enums;
using Wallet.Modules.user_module;
using Wallet.Tools.generic_module;

namespace Wallet.Modules.trade_module
{
    [Table("TRADE")]
    public class Trade : GenericModel
    {
        [Column("UserId")]
        [Display(Name = "User")]
        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Column("Type")]
        [Display(Name = "Type")]
        public eTradeType Type { get; set; }

        [Column("Date")]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Column("AssetId")]
        [Display(Name = "Asset")]
        public string? AssetId { get; set; }
        [ForeignKey("AssetId")]
        public Asset? Asset { get; set; }

        [Column("Amount")]
        [Display(Name = "Amount")]
        public string Amount { get; set; }

        [Column("Price")]
        [Display(Name = "Price")]
        public string Price { get; set; }
    }
}
