using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Web.Http.ModelBinding;
using Wallet.Modules.asset_module;
using Wallet.Modules.position_module;
using Wallet.Modules.user_module;
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
        private IUserService _userService;
        private readonly IPositionService _positionService;
        ModelStateDictionary modelState = new ModelStateDictionary();
        #endregion

        #region Constructor
        public TradeService(Context context, IUserService userService, IPositionService positionService)
        {
            _context = context;
            _userService = userService;
            _positionService = positionService;

            //Criar Triggers genéricos, o position precisa ser recalculado a cada alteração em trade.
            AfterInsert = async (obj) =>
            {
                var position = new Position
                {
                    AssetId = obj.AssetId,
                    CurrentPrice = 100,
                    AveragePrice = obj.Price,
                    UserId = obj.UserId,
                    Quantity = obj.Amount,
                    TotalGainLoss = 1000
                };
            };
        }
        #endregion

        public async Task<Trade> Creat(TradeDTO tradeDTO)
        {

            var userid = _userService.GetLoggedInUserId(); //tá dando erro aqui!
            var trade = new Trade()
            {
                UserId = userid,
                AssetId = tradeDTO.AssetId,
                Date = tradeDTO.Date,
                Type = tradeDTO.Type,
                Amount = tradeDTO.Amount,
                Price = tradeDTO.Price,
            };

            await InsertAsync(trade, _context);
            return trade;
        }

        public async Task<List<Trade>> Read(string? id = null)
        {
            var userId = _userService.GetLoggedInUserId();
            if (id == null) return await _context.Trade.AsQueryable().Where(a => a.UserId == userId).ToListAsync();
            return await _context.Trade.AsQueryable().Where(a => a.Id == id).ToListAsync();
        }

        public async Task<Trade> Update(Trade trade)
        {
            await UpdateAsync(trade, _context);
            return trade;
        }

        public async Task Delete(string id)
        {
            await DeleteAsync(id, _context);

        }
    }
}