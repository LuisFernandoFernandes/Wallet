using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Wallet.Modules.asset_module.enums
{
    public enum eAssetClass
    {
        #region Renda Variável Nacional


        [Display(Name = "Ação")]
        Acao = 0,

        [Display(Name = "ETF")]
        EtfBrasil = 1,

        [Display(Name = "FII")]
        Fii = 2,

        [Display(Name = "BDR")]
        Bdr = 3,

        #endregion

        #region Renda Variável Exterior

        [Display(Name = "Stock")]
        Stock = 4,

        [Display(Name = "ETF")]
        EtfExterior = 5,

        [Display(Name = "Reits")]
        Reits = 6,

        [Display(Name = "ADR")]
        ADR = 7,
        #endregion
    }
}
