using Microsoft.AspNetCore.Mvc;
using Wallet.Modules.asset_module;
using Wallet.Tools.database;

namespace Wallet.Modules.trade_module
{
    public class TradeController : ControllerBase
    {
        #region Variables
        private Context _context;
        private ITradeService _service;
        #endregion

        #region Constructor
        public TradeController(Context context)
        {
            _context = context;
            _service = new TradeService(_context);
        }
        #endregion
    }
}
