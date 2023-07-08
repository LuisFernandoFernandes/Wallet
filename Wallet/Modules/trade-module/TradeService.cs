using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Web.Http.ModelBinding;
using Wallet.Modules.asset_module;
using Wallet.Modules.position_module;
using Wallet.Modules.trade_module.enums;
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
            BeforeInsert = async (obj) => { InitBeforeInsert(obj); };
            AfterInsert = async (obj) => { await InitAfterInsert(obj); };
        }
        #endregion

        private void InitBeforeInsert(Trade obj)
        {
            if (obj.Type == eTradeType.Sell) obj.Amount = obj.Amount * -1;
        }

        private async Task InitAfterInsert(Trade obj)
        {
            await CalculatePosition(obj);
        }

        private async Task CalculatePosition(Trade obj)
        {
            var position = await _context.Position.AsQueryable().Where(a => a.UserId == obj.UserId && a.AssetId == obj.AssetId).FirstOrDefaultAsync();
            var currentPrice = await _context.Asset.AsQueryable().Where(a => a.Id == obj.AssetId).Select(a => a.Price).FirstOrDefaultAsync();

            if (position == null)
            {
                position = new Position
                {
                    AssetId = obj.AssetId,
                    AveragePrice = obj.Price,
                    UserId = obj.UserId,
                    Amount = obj.Amount,
                    TotalBought = obj.Type == eTradeType.Buy ? obj.Amount * obj.Price : 0,
                    TotalSold = obj.Type == eTradeType.Sell ? obj.Amount * obj.Price : 0,
                    TotalGainLoss = 0.0
                };
                await _positionService.InsertAsync(position, _context);
                return;
            }

            var newAmount = position.Amount + obj.Amount; //padronizar a nomenclatura.
            var newAveragePrice = ((position.Amount * position.AveragePrice) + (obj.Amount * obj.Price)) / newAmount;
            position.AveragePrice = newAveragePrice;
            position.Amount = newAmount;
            position.TotalBought = obj.Type == eTradeType.Buy ? position.TotalBought + (obj.Amount * obj.Price) : position.TotalBought;
            position.TotalSold = obj.Type == eTradeType.Sell ? position.TotalSold + (obj.Amount * obj.Price) : position.TotalSold;
            position.TotalGainLoss = position.TotalBought + position.TotalSold + position.Amount * currentPrice;

            await _positionService.UpdateAsync(position, _context);

            //Criar um scheduler que atualiza o current price dos assets contidos na positions dos user logados, algo assim.
        }

        public async Task<Trade> Creat(TradeDTO tradeDTO)
        {

            var userid = _userService.GetLoggedInUserId();
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
