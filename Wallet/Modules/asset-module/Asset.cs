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

        [Column("Price")]
        [Display(Name = "Price")]
        public double Price { get; set; } = 0.0; //adicionar no beforeInsert o valor atual quando inserir o ativo.
    }

    [Table("ASSET_HISTORICAL_DATA")]
    public class AssetHistoricalData : GenericModel
    {
        [Column("AssetId")]
        [Display(Name = "Asset")]
        public string? AssetId { get; set; }

        [ForeignKey("AssetId")]
        public Asset? Asset { get; set; }

        [Column("Date")]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Column("Open")]
        [Display(Name = "Open")]
        public double Open { get; set; }

        [Column("High")]
        [Display(Name = "High")]
        public double High { get; set; }

        [Column("Low")]
        [Display(Name = "Low")]
        public double Low { get; set; }

        [Column("Close")]
        [Display(Name = "Close")]
        public double Close { get; set; }

        [Column("AdjustedClose")]
        [Display(Name = "Adjusted Close")]
        public double AdjustedClose { get; set; }

        [Column("Volume")]
        [Display(Name = "Volume")]
        public long Volume { get; set; }

        [Column("DividendAmount")]
        [Display(Name = "Dividend Amount")]
        public double DividendAmount { get; set; }

        [Column("SplitCoefficient")]
        [Display(Name = "Split Coefficient")]
        public double SplitCoefficient { get; set; }
    }
}
