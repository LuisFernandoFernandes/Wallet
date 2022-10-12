using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;
using Wallet.Modules.asset_module.enums;
using Wallet.Tools.generic_module;

namespace Wallet.Modules.asset_module
{
    [Table("ASSET")]
    public class Asset : GenericModel
    {
        [Column("Class")]
        [Display(Name = "Class")]
        public eAssetClass? Class { get; set; }

        [Column("Ticker")]
        [Display(Name = "Ticker")]
        public string? Ticker { get; set; }

        [Column("Description")]
        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}
