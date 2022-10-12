using System.Web.Http.ModelBinding;
using Wallet.Modules.asset_module;
using Wallet.Tools.database;
using Wallet.Tools.generic_module;
using Wallet.Tools.validation_dictionary;

namespace Wallet.Modules.trade_module
{
    internal class TradeService : GenericService<Trade>, ITradeService
    {
        #region Variables
        private IValidationDictionary _validatonDictionary;
        private TradeRepository _repository;
        private Context _context;


        ModelStateDictionary modelState = new ModelStateDictionary();
        #endregion

        #region Constructor
        public TradeService(Context context)
        {
            _context = context;
        }

        #endregion
    }
}