﻿using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Web.Http.ModelBinding;
using Wallet.Modules.asset_module;
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
        ModelStateDictionary modelState = new ModelStateDictionary();
        #endregion

        #region Constructor
        public TradeService(Context context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<Trade> Creat(Trade trade)
        {
            trade.UserId = _userService.GetLoggedInUserId();
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
        }

        public async Task<Trade> Delete(string id)
        {
            await DeleteAsync(id, _context);

        }





        #endregion
    }
}