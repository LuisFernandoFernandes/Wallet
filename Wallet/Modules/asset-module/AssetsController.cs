using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Tools.alpha_vantage;
using Wallet.Tools.database;

namespace Wallet.Modules.asset_module
{
    [Authorize]
    [Route("assets")]
    public class AssetsController : ControllerBase
    {
        #region Vars
        private Context _context;
        private readonly IAlphaVantageService _alphaVantageService;
        private IAssetService _service;
        #endregion

        #region Constructor
        public AssetsController(Context context, IAlphaVantageService alphaVantageService)
        {
            _context = context;
            _alphaVantageService = alphaVantageService;
            _service = new AssetService(_context, alphaVantageService);
        }
        #endregion

        #region Creat
        [HttpPost]
        public async Task<ActionResult<string>> Creat(Asset asset)
        {
            try
            {
                var response = await _service.Creat(asset);
                return Ok(response);
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
        #endregion

        #region Read
        [HttpGet]
        public async Task<ActionResult<Asset>> Read(string? id, string? ticker)
        {
            try
            {
                var list = await _service.Read(id, ticker);
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
        public async Task<ActionResult<string>> Update(string id, Asset asset)
        {
            try
            {
                var response = await _service.Update(id, asset);
                return Ok(response);
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
                return NotFound("Ativo já cadastrado.");
            }
            catch (Exception)
            {
                return Problem("Algo deu errado, contate o administrador.");
            }
        }
        #endregion
    }
}
