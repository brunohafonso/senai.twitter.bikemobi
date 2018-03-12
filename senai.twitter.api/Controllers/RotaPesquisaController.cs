using System;
using System.Linq;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using senai.twitter.domain.Contracts;
using senai.twitter.domain.Entities;

namespace senai.twitter.api.Controllers
{
    [Route("api/[Controller]")]
    public class RotaPesquisaController : Controller
    {
        private IBaseRepository<Login> _loginRepository;
        private IBaseRepository<Perfil> _perfilRepository;
        private IBaseRepository<RotaPesquisa> _rotaPesquisaRepository;

        public RotaPesquisaController(IBaseRepository<Login> loginRepository,IBaseRepository<Perfil> perfilRepository, IBaseRepository<RotaPesquisa> rotaPesquisaRepository)
        {

            _loginRepository = loginRepository;
            _perfilRepository = perfilRepository;
            _rotaPesquisaRepository = rotaPesquisaRepository;
        }
        
        
        /// <summary>
        /// Busca todas as pesquisas na base de dados
        /// </summary>
        /// <returns>Lista com todas as pesquisas realizadas.</returns>
        [Route("todos")]
        [HttpGet]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult Buscar()
        {
            try
            {
                var rotas = _rotaPesquisaRepository.Listar();
                return Ok(rotas);
            }
            catch(Exception ex)
            {
                return BadRequest("erro ao buscar dados " + ex.Message);
            }
        }

        /// <summary>
        /// Efetua a busca das rotas pesquisadas por um usuário X
        /// </summary>
        /// <param name="id">Id do login do serão buscadas as rotas pesquisadas</param>
        /// <returns>Objeto buscado caso exista algum registro com Id persquisado</returns>
        [Route("buscarid/{id}")]
        [HttpGet]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult BuscarPorId(int id)
        {
            var rotas = _rotaPesquisaRepository.Listar().Where(c => c.IdLogin == id);
            if (rotas.Count() > 0)
                return Ok(rotas);
            else
                return NotFound("não existe nenhuma rota no perfil pesquisado.");
        }

        /// <summary>
        /// Efetua o cadastro de rotas pesquisadas
        /// </summary>
        /// <param name="rota">Dados da rota conforme criterios estabelecidos (precisa receber o objeto inteiro)</param>
        /// <returns>String informando qual objeto foi cadastrado.</returns>
        [Route("cadastrar")]
        [HttpPost]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult Cadastrar([FromBody] RotaPesquisa rota)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            try 
            {
                rota.CriadoEm = DateTime.Now;
                rota.QtdAtualizacoes = 0;
                rota.AtualizadoPor = null;
                
                _rotaPesquisaRepository.Inserir(rota);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest("Erro ao cadastrar pesquisa de rota. " + ex.Message);
            }
            
        }
    }
}