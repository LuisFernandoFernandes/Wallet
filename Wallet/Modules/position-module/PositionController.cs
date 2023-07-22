using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Modules.position_module;
using Wallet.Modules.user_module;
using Wallet.Tools.database;

namespace Wallet.Modules.position_module
{
    [Authorize]
    [Route("position")]
    public class PositionController : ControllerBase
    {
        #region Vars
        private Context _context;
        private IPositionService _service;
        private IUserService _userService;
        #endregion

        #region Constructor
        public PositionController(Context context, IUserService userService)
        {
            _context = context;
            _userService = userService;
            _service = new PositionService(_context, _userService);
        }
        #endregion


        #region Read
        [HttpGet]
        public async Task<ActionResult<List<PositionDTO>>> Read()
        {
            try
            {
                var list = await _service.Read();
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
    }
}
