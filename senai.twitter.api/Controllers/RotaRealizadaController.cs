using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
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
        /// Busca todas as rotas realizadas na base de dados.
        /// </summary>
        /// <remarks>    
        /// Exemplo de Retorno:
        ///     
        ///     GET http://brunohafonso-001-site1.ctempurl.com/api/RotaRealizada/todos    
        /// 
        ///     {
        ///         "latInicio": 0,
        ///         "lngInicio": 0,
        ///         "latFim": 0,
        ///         "lngFim": 0,
        ///         "duracaoString": "string",
        ///         "duracaoInt": 0,
        ///         "kilometros": 0,
        ///         "login": null,
        ///         "idLogin": 0,
        ///         "rotaPesquisada": {
        ///             "distancia": 0,
        ///             "duracao": "string",
        ///             "destinoEnd": "string",
        ///             "destinoLat": 0,
        ///             "destinoLng": 0,
        ///             "origemEnd": "string",
        ///             "origemLat": 0,
        ///             "origemLng": 0,
        ///             "polylinePoints": "string",
        ///             "login": null,
        ///             "idLogin": 0,
        ///             "id": 0,
        ///             "atualizadoEm": "0001-01-01T00:00:00",
        ///             "atualizadoPor": null,
        ///             "criadoEm": "2018-03-25T16:11:23.9309983",
        ///             "qtdAtualizacoes": 0
        ///         },
        ///         "idRotaPesquisada": 0,
        ///         "avaliacao": {
        ///             "avTrajeto": 0,
        ///             "avSeguranca": 0,
        ///             "idRotaRealizada": 0,
        ///             "login": null,
        ///             "idLogin": 0,
        ///             "id": 0,
        ///             "atualizadoEm": "0001-01-01T00:00:00",
        ///             "atualizadoPor": null,
        ///             "criadoEm": "2018-03-25T16:11:23.9330105",
        ///             "qtdAtualizacoes": 0
        ///         },
        ///         "id": 0,
        ///         "criadoEm": "2018-03-25T16:11:23.9320365",
        ///         "atualizadoEm": "0001-01-01T00:00:00",
        ///         "qtdAtualizacoes": 0,
        ///         "atualizadoPor": null
        ///     }
        /// 
        /// </remarks>
        /// <returns>Lista todas as rotas realizadas.</returns>
        /// <response code="200"> Retorna lista com todas as rotas realizadas na base de dados.</response>
        /// <response code="400"> Ocorreu um erro.</response>
        /// <response code="404"> Nenhuma rota realizada cadastrada.</response>
        [HttpGet]
        [Route("todos")]
        [EnableCors("AllowAnyOrigin")]
        [Authorize("Bearer")]
        public IActionResult Buscar()
        {
           try
           {
               var rotas = _rotaRealizadaRepository.Listar(new string[]{"RotaPesquisada","Avaliacao"});
                if(rotas.Count() < 1) return NotFound("Não existe nenhuma rota realizada cadastrada.");

               return Ok(rotas);
           } 
           catch(Exception ex)
           {
               return BadRequest("Erro ao listar dados. " + ex.Message);
           }
        }

        /// <summary>
        /// Efetua a busca das rotas realizadas por um usuário com o id pesquisado.
        /// </summary>
        /// <remarks>    
        /// Exemplo de Retorno:
        ///     
        ///     GET http://brunohafonso-001-site1.ctempurl.com/api/RotaRealizada/buscarid/{Id}   
        /// 
        ///     {
        ///         "latInicio": 0,
        ///         "lngInicio": 0,
        ///         "latFim": 0,
        ///         "lngFim": 0,
        ///         "duracaoString": "string",
        ///         "duracaoInt": 0,
        ///         "kilometros": 0,
        ///         "login": null,
        ///         "idLogin": 0,
        ///         "rotaPesquisada": {
        ///             "distancia": 0,
        ///             "duracao": "string",
        ///             "destinoEnd": "string",
        ///             "destinoLat": 0,
        ///             "destinoLng": 0,
        ///             "origemEnd": "string",
        ///             "origemLat": 0,
        ///             "origemLng": 0,
        ///             "polylinePoints": "string",
        ///             "login": null,
        ///             "idLogin": 0,
        ///             "id": 0,
        ///             "atualizadoEm": "0001-01-01T00:00:00",
        ///             "atualizadoPor": null,
        ///             "criadoEm": "2018-03-25T16:11:23.9309983",
        ///             "qtdAtualizacoes": 0
        ///         },
        ///         "idRotaPesquisada": 0,
        ///         "avaliacao": {
        ///             "avTrajeto": 0,
        ///             "avSeguranca": 0,
        ///             "idRotaRealizada": 0,
        ///             "login": null,
        ///             "idLogin": 0,
        ///             "id": 0,
        ///             "atualizadoEm": "0001-01-01T00:00:00",
        ///             "atualizadoPor": null,
        ///             "criadoEm": "2018-03-25T16:11:23.9330105",
        ///             "qtdAtualizacoes": 0
        ///         },
        ///         "id": 0,
        ///         "criadoEm": "2018-03-25T16:11:23.9320365",
        ///         "atualizadoEm": "0001-01-01T00:00:00",
        ///         "qtdAtualizacoes": 0,
        ///         "atualizadoPor": null
        ///     }
        /// 
        /// </remarks>
        /// <param name="Id">Id do login que serão buscadas as rotas realizadas.</param>
        /// <returns>Objeto buscado caso exista algum registro com Id persquisado.</returns>
        /// <response code="200"> Retorna lista com todas as rotas realizadas na base de dados com base no Id pesquisado.</response>
        /// <response code="404"> Nenhuma rota realizada cadastrada com esse perfil pesquisado.</response>
        [HttpGet]
        [Route("buscarid/{Id}")]
        [EnableCors("AllowAnyOrigin")]
        [Authorize("Bearer")]
        public IActionResult buscarid(int Id)
        {
            var rotas = _rotaRealizadaRepository.Listar(new string[]{"RotaPesquisada", "Avaliacao"}).Where(r => r.IdLogin == Id);
            if (rotas.Count() > 0)
                return Ok(rotas);
            else
                return NotFound("não existe nenhuma rota no perfil pesquisado.");
        }

        /// <summary>
        /// Efetua o cadastro de rotas realizadas
        /// </summary>
        /// <remarks>    
        /// Exemplo de Requisição:
        ///     
        ///     POST http://brunohafonso-001-site1.ctempurl.com/api/RotaRealizada/cadastrar  
        /// 
        ///     {
        ///         "latInicio": 0,
        ///         "lngInicio": 0,
        ///         "latFim": 0,
        ///         "lngFim": 0,
        ///         "duracaoString": "string",
        ///         "duracaoInt": 0,
        ///         "kilometros": 0,
        ///         "idLogin": 0,
        ///         "idRotaPesquisada": 0
        ///     }
        /// 
        /// </remarks>
        /// <param name="rota">Dados da rota conforme criterios estabelecidos (precisa receber o objeto inteiro)</param>
        /// <returns>String informando se o objeto foi cadastrado.</returns>
        /// <response code="200"> Retorna objeto com string informando que rota foi salva com sucesso e o Id do ultimo registro cadastrado..</response>
        /// <response code="400"> Ocorreu um erro.</response>
        [Route("cadastrar")]
        [HttpPost]
        [EnableCors("AllowAnyOrigin")]
        [Authorize("Bearer")]
        public IActionResult Cadastrar([FromBody] RotaRealizada rota)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            try 
            {
                rota.CriadoEm = DateTime.Now;
                rota.QtdAtualizacoes = 0;
                rota.AtualizadoPor = null;
                
                 _rotaRealizadaRepository.Inserir(rota);
                var retorno = new {
                    ultimoIdCadastrado = rota.Id,
                    mensagem = "Rota salva com sucesso."
                };

                return Ok(retorno);
            }
            catch(Exception ex)
            {
                return BadRequest("Erro ao cadastrar rota realizada. " + ex.Message);
            }
        }
    }
}