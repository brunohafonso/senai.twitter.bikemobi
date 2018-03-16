using System;
using System.Linq;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using senai.twitter.domain.Contracts;
using senai.twitter.domain.Entities;

namespace senai.twitter.api.Controllers
{
    [Route("api/[controller]")]
    public class RotaRealizadaController : Controller
    {
        private IBaseRepository<Login> _loginRepository;
        private IBaseRepository<Perfil> _perfilRepository;
        private IBaseRepository<RotaPesquisada> _rotaPesquisadaRepository;

        private IBaseRepository<RotaRealizada> _rotaRealizadaRepository;

        public RotaRealizadaController(IBaseRepository<Login> loginRepository,IBaseRepository<Perfil> perfilRepository, IBaseRepository<RotaPesquisada> rotaPesquisadaRepository,IBaseRepository<RotaRealizada> rotaRealizadaRepository)
        {
            _loginRepository = loginRepository;
            _perfilRepository = perfilRepository;
            _rotaPesquisadaRepository = rotaPesquisadaRepository;
            _rotaRealizadaRepository = rotaRealizadaRepository;
        }

        /// <summary>
        /// Busca todas as rotas realizadas na base de dados
        /// </summary>
        /// <returns>Lista todas as rotas realizadas.</returns>
        [HttpGet]
        [Route("todos")]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult Buscar()
        {
           try
           {
               var rotas = _rotaRealizadaRepository.Listar(new string[]{"RotaPesquisada"});
               return Ok(rotas);
           } 
           catch(Exception ex)
           {
               return BadRequest("Erro ao listar dados. " + ex.Message);
           }
        }

        /// <summary>
        /// Efetua a busca das rotas realizadas por um usuário com o id pesquisado
        /// </summary>
        /// <param name="id">Id do login que serão buscadas as rotas realizadas</param>
        /// <returns>Objeto buscado caso exista algum registro com Id persquisado</returns>
        [HttpGet]
        [Route("buscarid/{Id}")]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult buscarid(int Id)
        {
            var rotas = _rotaRealizadaRepository.Listar(new string[]{"RotaPesquisada"}).Where(r => r.IdLogin == Id);
            if (rotas.Count() > 0)
                return Ok(rotas);
            else
                return NotFound("não existe nenhuma rota no perfil pesquisado.");
        }
    }
}