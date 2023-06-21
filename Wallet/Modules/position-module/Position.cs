using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wallet.Modules.asset_module;
using Wallet.Modules.user_module;
using Wallet.Tools.generic_module;

namespace Wallet.Modules.position_module
{
    [Table("POSITION")]
    public class Position : GenericModel
    {
        [Column("UserId")]
        [Display(Name = "User")]
        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Column("AssetId")]
        [Display(Name = "Asset")]
        public string? AssetId { get; set; }

        [ForeignKey("AssetId")]
        public Asset? Asset { get; set; }

        [Column("Quantity")]
        [Display(Name = "Quantity")]
        public string Quantity { get; set; }

        [Column("AveragePrice")]
        [Display(Name = "Average Price")]
        public string AveragePrice { get; set; }


        [Display(Name = "Current Price")]
        [Column("CurrentPrice")]
        public string CurrentPrice { get; set; }


        [Display(Name = "Total Gain/Loss")]
        [Column("TotalGainLoss")]
        public string TotalGainLoss { get; set; }
    }
}
