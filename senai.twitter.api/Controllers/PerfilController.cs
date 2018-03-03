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

        /// <summary>
        /// Busca todos os registros de perfis cadastrados
        /// </summary>
        /// <returns>lista de perfis cadastrados</returns>
        [HttpGet]
        [Route("todos")]
        public IActionResult Buscar()
        {
            try {
                 return Ok(_perfilRepository.Listar());
            } 
            catch(Exception ex)
            {
                return BadRequest("Erro ao buscar dados. " + ex.Message);
            }  
        }

        /// <summary>
        /// Efetua a atualização dos dados do Login juntamente com os dados básicos do perfil
        /// </summary>
        /// <param name="Id">Id do perfil a ser buscad na base de dados</param>
        /// <returns>Objeto buscado caso exista algum registro com  Id persquisado</returns>
        [HttpGet]
        [Route("buscarid/{Id}")]
        public IActionResult BuscarPorId(int Id)
        {
            try
            {
                var perfil = _perfilRepository.BuscarPorId(Id);
                if(perfil != null)
                    return Ok(perfil);
                else
                    return NotFound();
            }
            catch(Exception ex)
            {
                return BadRequest("Errp ao buscar dados. " + ex.Message);
            }
        }

        /// <summary>
        /// Efetua a atualização dos dados do Login juntamente com os dados básicos do perfil
        /// </summary>
        /// <param name="perfil">Dados do login/perfil conforme criterios estabelecidos (precisa receber o objeto inteiro)</param>
        /// <returns>String informando qual objeto foi atualizado.</returns>
        [HttpPut]
        [Route("atualizar")]
        public IActionResult Atualizar([FromBody] Perfil perfil)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            try
            {
                 _perfilRepository.Atualizar(perfil);
                 return Ok($"Usuário {perfil.Nome} Atualizado Com Sucesso.");

            }
            catch (Exception ex)
            {
                return BadRequest("Erro ao atualizar os dados. " + ex.Message);
            }
        }
    }
}