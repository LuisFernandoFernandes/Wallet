using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wallet.Modules.position_module;
using Wallet.Modules.trade_module;
using Wallet.Modules.user_module;
using Wallet.Tools.database;

namespace Wallet.Modules.trade_module
{
    public class TradeController : ControllerBase
    {
        #region Variables
        private Context _context;
        private readonly IUserService _userService;
        private ITradeService _service;
        private IUserService _userService;
        private IPositionService _positionService;
        #endregion

        #region Constructor
        public TradeController(Context context, IUserService userService)
        {
            _context = context;
            _service = new TradeService(_context);
        }
        #endregion


        #region Creat
        [HttpPost]
        public async Task<ActionResult<string>> Creat(Trade trade)
        {
            try
            {
                var response = await _service.Creat(trade);
                return Ok(response);
            }
            catch (ArgumentNullException)
            {
                return NotFound("Movimentação já cadastrado.");
            }
            catch (Exception)
            {
                return Problem("Algo deu errado, contate o administrador.");
            }
        }
        #endregion

        #region Read
        [HttpGet]
        public async Task<ActionResult<Trade>> Read(string? id)
        {
            try
            {
                var list = await _service.Read(id);
                return Ok(list);
            }
            catch (ArgumentNullException)
            {
                return NotFound("Nenhum ativo encontrado.");
            }
            catch (Exception)
            {
                return Problem("Algo deu errado, contate o administrador.");
            }
        }
        #endregion

        #region Update
        [HttpPatch]
        public async Task<ActionResult<string>> Update(Trade trade)
        {
            try
            {
                var response = await _service.Update(trade);
                return Ok(response);
            }
            catch (ArgumentNullException)
            {
                return NotFound("Movimentação já cadastrado.");
            }
            catch (Exception)
            {
                return Problem("Algo deu errado, contate o administrador.");
            }
        }
        #endregion

        #region Delete
        [HttpDelete]
        public async Task<ActionResult<string>> Delete(string id)
        {
            try
            {
                var response = await _service.Delete(id);
                return Ok(response);
            }
            catch (ArgumentNullException)
            {
                return NotFound("Movimentação já cadastrado.");
            }
            catch (Exception)
            {
                return Problem("Algo deu errado, contate o administrador.");
            }
        }
        #endregion
    }
}
