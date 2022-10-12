using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using System.Web.Http;

namespace Wallet.Tools.generic_module
{
    public class GenericController<TModelo> : ControllerBase where TModelo : class, IGenericModel
    {

        public IGenericService<TModelo> _service;

        public GenericController(IGenericService<TModelo> service)
        {
            _service = service;
            Thread.CurrentPrincipal = User;
        }


        public IActionResult GenericResult<T>(IGenericResult<T> response, Func<IGenericResult<T>, object> minify = null)
        {
            return Ok(minify != null ? minify(response) : response);
        }

        public IActionResult OkResult<T>(T response, Func<IGenericResult<T>, object> minify = null)
        {
            return GenericResult(new OkResult<T>(response, null), minify);
        }

        public IActionResult InvalidResult(string response, Func<IGenericResult<TModelo>, object> minify = null)
        {
            return GenericResult(new InvalidResult<TModelo>(response), minify);
        }

        #region CRUD
        [HttpPost] //Create
        public virtual async Task<IActionResult> PostIncluirAsync([FromBody] TModelo obj)
        {
            await _service.IncluirAsync(obj);
            return OkResult(obj);
        }

        [HttpGet]
        [Route("{id}")] //GetOne
        public virtual async Task<IActionResult> GetOne(string id)
        {
            return OkResult(await _service.AsQueryable().Where(a => a.Id == id).FirstOrDefaultAsync());
        }


        [HttpGet] //GetAll
        public virtual async Task<IActionResult> Get()
        {
            return OkResult(await _service.AsQueryable().ToListAsync());
        }


        [HttpPatch] //Update
        public virtual async Task<IActionResult> Update([FromBody] TModelo obj)
        {
            await _service.AlterarAsync(obj);
            return OkResult(obj);
        }


        [HttpDelete]
        [Route("{id}")] //Delete
        public virtual async Task<IActionResult> Delete(string id)
        {
            await _service.ExcluirAsync(id);
            return OkResult<TModelo>(null);
        }

        #endregion

    }
}
