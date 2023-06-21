using Microsoft.AspNetCore.Mvc;
using Wallet.Modules.position_module;
using Wallet.Modules.user_module;
using Wallet.Tools.database;

namespace Wallet.Modules.position_module
{
    public class PositionController : ControllerBase
    {
        #region Variables
        private Context _context;
        private IPositionService _service;
        private IUserService _userService;
        #endregion

        #region Constructor
        public PositionController(Context context)
        {
            _context = context;
            _service = new PositionService(_context);
        }
        #endregion


        //#region Creat
        //[HttpPost]
        //public async Task<ActionResult<string>> Creat(Position position)
        //{
        //    try
        //    {
        //        var response = await _service.Creat(position);
        //        return Ok(response);
        //    }
        //    catch (ArgumentNullException)
        //    {
        //        return NotFound("Movimentação já cadastrado.");
        //    }
        //    catch (Exception)
        //    {
        //        return Problem("Algo deu errado, contate o administrador.");
        //    }
        //}
        //#endregion

        #region Read
        [HttpGet]
        public async Task<ActionResult<Position>> Read(string? id)
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

        //#region Update
        //[HttpPatch]
        //public async Task<ActionResult<string>> Update(Position position)
        //{
        //    try
        //    {
        //        var response = await _service.Update(position);
        //        return Ok(response);
        //    }
        //    catch (ArgumentNullException)
        //    {
        //        return NotFound("Movimentação já cadastrado.");
        //    }
        //    catch (Exception)
        //    {
        //        return Problem("Algo deu errado, contate o administrador.");
        //    }
        //}
        //#endregion

        //#region Delete
        //[HttpDelete]
        //public async Task<ActionResult<string>> Delete(string id)
        //{
        //    try
        //    {
        //        await _service.Delete(id);
        //        return Ok("Movimentação excluída com sucesso.");
        //    }
        //    catch (ArgumentNullException)
        //    {
        //        return NotFound("Movimentação já cadastrado.");
        //    }
        //    catch (Exception)
        //    {
        //        return Problem("Algo deu errado, contate o administrador.");
        //    }
        //}
        //#endregion
    }
}
