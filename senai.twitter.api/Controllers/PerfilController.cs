using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using senai.twitter.domain.Contracts;
using senai.twitter.domain.Entities;
using senai.twitter.repository.Context;

namespace senai.twitter.api.Controllers
{
    [Route("api/[controller]")]
    public class PerfilController : Controller
    {
        private IBaseRepository<Login> _loginRepository;
        private IBaseRepository<Perfil> _perfilRepository;

    

        public PerfilController(IBaseRepository<Login> loginRepository,IBaseRepository<Perfil> perfilRepository)
        {
            _loginRepository = loginRepository;
            _perfilRepository = perfilRepository;
        }

        [HttpGet]
        public IActionResult Buscar()
        {
            try {
                 return Ok("_perfilRepository.Listar()");
            } 
            catch(Exception ex)
            {
                return BadRequest("Erro ao buscar dados. " + ex.Message);
            }  
        }

        [HttpGet]
        [Route("buscarid/{Id}")]
        public IActionResult BuscarPorId(int Id)
        {
            var perfil = _perfilRepository.BuscarPorId(Id);
            if(perfil != null)
                return Ok(perfil);
            else
                return NotFound();
        }

        [HttpPut]
        [Route("atualizar")]
        public IActionResult Atualizar([FromBody] Perfil perfil)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            return Ok(_perfilRepository.Atualizar(perfil));
        }
    }
}