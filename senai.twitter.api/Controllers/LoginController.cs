using System;
using Microsoft.AspNetCore.Mvc;
using senai.twitter.domain.Contracts;
using senai.twitter.domain.Entities;

namespace senai.twitter.api.Controllers
{
    [Route("api/cadastro")]
    public class LoginController : Controller
    {
        private IBaseRepository<Login> _loginRepository;
        private IBaseRepository<Perfil> _perfilRepository;

        public LoginController(IBaseRepository<Login> loginRepository,IBaseRepository<Perfil> perfilRepository)
        {

            _loginRepository = loginRepository;
            _perfilRepository = perfilRepository;
        }

        // [HttpGet]
        // public IActionResult Buscar()
        // {
        //     return Ok(_loginRepository.Listar(new string[]{"Perfil"}));
        // }

        // [HttpGet("{id}")]
        // public IActionResult BuscarPorId(int id)
        // {
        //     var login = _loginRepository.BuscarPorId(id);
        //     var perfil = _perfilRepository.BuscarPorId(id);
        //     login.Perfil = perfil;
        //     if (login != null)
        //         return Ok(login);
        //     else
        //         return NotFound();
        // }

       [Route("cadastrar")]
        [HttpPost]
        public IActionResult Cadastrar([FromBody] Login login)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            try 
            {
                _loginRepository.Inserir(login);
                return Ok($"Usuário {login.NomeUsuario} Cadastrado Com Sucesso.");
            }
            catch(Exception ex)
            {
                return BadRequest("Erro ao cadastrar dados. " + ex.Message);
            }
            
        }
        
        [Route("atualizar")]
        [HttpPut]
        public IActionResult Atualizar([FromBody] Login login)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _loginRepository.Atualizar(login);
                return Ok($"Usuário {login.NomeUsuario} Atualizado Com Sucesso.");
            }
            catch(Exception ex)
            {
                return BadRequest("Erro ao atualizar dados. " + ex.Message);
            }
        }

        [Route("deletar")]
        [HttpDelete]
        public IActionResult Deletar([FromBody] Login login)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            try
            {
                 _loginRepository.Deletar(login);
                 return Ok($"Usuário {login.NomeUsuario} Deletado Com Sucesso.");
            }
            catch(Exception ex)
            {
                return BadRequest("Erro ao deletar dados. " + ex.Message);
            }
        }
    }
}