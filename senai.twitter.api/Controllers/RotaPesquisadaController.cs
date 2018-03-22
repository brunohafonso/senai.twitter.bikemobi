using System;
using System.Linq;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using senai.twitter.domain.Contracts;
using senai.twitter.domain.Entities;

namespace senai.twitter.api.Controllers
{
    [Route("api/[Controller]")]
    public class RotaPesquisadaController : Controller
    {
        private IBaseRepository<Login> _loginRepository;
        private IBaseRepository<Perfil> _perfilRepository;
        private IBaseRepository<RotaPesquisada> _rotaPesquisadaRepository;

        private IBaseRepository<RotaRealizada> _rotaRealizadaRepository;

        public RotaPesquisadaController(IBaseRepository<Login> loginRepository,IBaseRepository<Perfil> perfilRepository, IBaseRepository<RotaPesquisada> rotaPesquisadaRepository,IBaseRepository<RotaRealizada> rotaRealizadaRepository)
        {
            _loginRepository = loginRepository;
            _perfilRepository = perfilRepository;
            _rotaPesquisadaRepository = rotaPesquisadaRepository;
            _rotaRealizadaRepository = rotaRealizadaRepository;
        }
        
        
        /// <summary>
        /// Busca todas as pesquisas na base de dados
        /// </summary>
        /// <returns>Lista todas as pesquisas realizadas.</returns>
        [Route("todos")]
        [HttpGet]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult Buscar()
        {
            try
            {
                var rotas = _rotaPesquisadaRepository.Listar(new string[]{"RotaRealizada"});
                return Ok(rotas);
            }
            catch(Exception ex)
            {
                return BadRequest("erro ao buscar dados " + ex.Message);
            }
        }

        /// <summary>
        /// Efetua a busca das rotas pesquisadas por um usuário com o id pesquisado
        /// </summary>
        /// <param name="Id">Id do login que serão buscadas as rotas pesquisadas</param>
        /// <returns>Objeto buscado caso exista algum registro com Id persquisado</returns>
        [Route("buscarid/{Id}")]
        [HttpGet]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult BuscarPorId(int Id)
        {
            var rotas = _rotaPesquisadaRepository.Listar(new string[]{"RotaRealizada"}).Where(c => c.IdLogin == Id);
            if (rotas.Count() > 0)
                return Ok(rotas);
            else
                return NotFound("não existe nenhuma rota no perfil pesquisado.");
        }

        /// <summary>
        /// Efetua o cadastro de rotas pesquisadas
        /// </summary>
        /// <param name="rota">Dados da rota conforme criterios estabelecidos (precisa receber o objeto inteiro)</param>
        /// <returns>String informando se o objeto foi cadastrado.</returns>
        [Route("cadastrar")]
        [HttpPost]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult Cadastrar([FromBody] RotaPesquisada rota)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            try 
            {
                rota.CriadoEm = DateTime.Now;
                rota.QtdAtualizacoes = 0;
                rota.AtualizadoPor = null;
                
                _rotaPesquisadaRepository.Inserir(rota);
                return Ok("Rota salva com sucesso.");
            }
            catch(Exception ex)
            {
                return BadRequest("Erro ao cadastrar pesquisa de rota. " + ex.Message);
            }
        }
    }
}