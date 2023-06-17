﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wallet.Tools.database;
using Wallet.Tools.session_control;

namespace Wallet.Modules.user_module
{
    [Route("user")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        #region Variables
        private Context _context;
        private readonly IConfiguration _configuration;
        private IUserService _service;
        #endregion

        #region Constructor
        public UserController(Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _service = new UserService(context, configuration);
        }
        #endregion

        #region Creat
        [HttpPost, AllowAnonymous]
        public async Task<ActionResult<string>> Creat(UserDTO user)
        {
            try
            {
                var response = await _service.Creat(user);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                //return Problem("Algo deu errado, contate o administrador.");
            }
        }
        #endregion

        #region Read
        [HttpGet]
        public async Task<ActionResult<User>> Read(string? id, string? ticker)
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
        public async Task<ActionResult<string>> Update(string id, User user)
        {
            try
            {
                var response = await _service.Update(id, user);
                return Ok(response);
            }
            catch (ArgumentNullException ex)
            {
                var msg = string.IsNullOrEmpty(ex.Message) ? "Usuário já cadastrado." : ex.Message;
                return NotFound(msg);
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
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Pega o Id do usuário que fez a requisição.

                var response = await _service.Delete(id);
                return Ok(response);
            }
            catch (ArgumentNullException)
            {
                return NotFound("Usuário já cadastrado.");
            }
            catch (Exception)
            {
                return Problem("Algo deu errado, contate o administrador.");
            }
        }
        #endregion

        #region Login
        [HttpPost("login"), AllowAnonymous]
        public async Task<ActionResult<string>> Login(UserDTO userDTO)
        {
            try
            {
                var response = await _service.Login(userDTO);
                return Ok(response);

            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {

                return Problem("Erro desconhecido, contate o administrador.");
            }
        }
        #endregion



        [HttpGet("gerarcpf"), AllowAnonymous]
        public ActionResult<string> GerarCpf()
        {
            try
            {
                return Ok(_service.CreateRandomCpf());
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return Problem("Algo deu errado, contate o administrador.");
            }
        }

    }
}
