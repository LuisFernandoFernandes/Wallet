using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Modules.asset_module;
using Wallet.Tools.alpha_vantage;
using Wallet.Tools.database;

namespace Wallet.Modules.adm_module
{
    [Authorize(Policy = "RequireAdminRole")]
    [Route("adm")]
    public class AdmController : ControllerBase
    {


        #region Variables
        #endregion

        #region Constructor
        public AdmController()
        {
        }
        #endregion

        [HttpPost]
        public async Task<ActionResult<string>> Creat()
        {
            try
            {
                // var response = await _service.Creat(asset);
                return Ok("OK");
            }
            catch (ArgumentNullException)
            {
                return NotFound("Ativo já cadastrado.");
            }
            catch (Exception)
            {
                return Problem("Algo deu errado, contate o administrador.");
            }
        }
    }
}
