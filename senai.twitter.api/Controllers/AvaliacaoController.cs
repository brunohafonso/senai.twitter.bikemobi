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
    public class AvaliacaoController : Controller
    {
        private IBaseRepository<Login> _loginRepository;
        private IBaseRepository<Perfil> _perfilRepository;
        private IBaseRepository<RotaPesquisada> _rotaPesquisadaRepository;

        private IBaseRepository<RotaRealizada> _rotaRealizadaRepository;

        private IBaseRepository<Avaliacao> _avaliacaoRepository;

        public AvaliacaoController(IBaseRepository<Login> loginRepository,IBaseRepository<Perfil> perfilRepository, IBaseRepository<RotaPesquisada> rotaPesquisadaRepository,IBaseRepository<RotaRealizada> rotaRealizadaRepository, IBaseRepository<Avaliacao> avaliacaoRepository)
        {
            _loginRepository = loginRepository;
            _perfilRepository = perfilRepository;
            _rotaPesquisadaRepository = rotaPesquisadaRepository;
            _rotaRealizadaRepository = rotaRealizadaRepository;
            _avaliacaoRepository = avaliacaoRepository;
        }

        /// <summary>
        /// Busca todas as avaliações na base de dados.
        /// </summary>
        /// <remarks>
        /// Exemplo de Retorno:
        ///
        ///     GET http://brunohafonso-001-site1.ctempurl.com/api/Avaliacao/todos
        ///    
        ///     {
        ///         "avTrajeto": 0,
        ///         "avSeguranca": 0,
        ///         "rotaRealizada": {
        ///             "latInicio": 0,
        ///             "lngInicio": 0,
        ///             "latFim": 0,
        ///             "lngFim": 0,
        ///             "duracaoString": "string",
        ///             "duracaoInt": 0,
        ///             "kilometros": 0,
        ///             "login": null,
        ///             "idLogin": 0,
        ///             "rotaPesquisada": null,
        ///             "idRotaPesquisada": 1,
        ///             "id": 0,
        ///             "atualizadoEm": "0001-01-01T00:00:00",
        ///             "atualizadoPor": null,
        ///             "criadoEm": "2018-03-25T16:11:23.9320365",
        ///             "qtdAtualizacoes": 0
        ///         },
        ///         "idRotaRealizada": 0,
        ///         "login": null,
        ///         "idLogin": 0,
        ///         "id": 0,
        ///         "criadoEm": "2018-03-25T16:11:23.9330105",
        ///         "atualizadoEm": "0001-01-01T00:00:00",
        ///         "qtdAtualizacoes": 0,
        ///         "atualizadoPor": null
        ///     }
        /// 
        /// </remarks>
        /// <returns>Lista todas as avaliações realizadas.</returns>
        /// <response code="200"> Retorna lista com todas as avaliações realizadas.</response>
        /// <response code="400"> Ocorreu um erro.</response>
        /// <response code="404"> Nenhuma avaliação cadastrada.</response>
        [Route("todos")]
        [HttpGet]
        [EnableCors("AllowAnyOrigin")]
        [AllowAnonymous]
        public IActionResult Buscar()
        {
            try
            {
                var avaliacoes = _avaliacaoRepository.Listar(new string[]{"RotaRealizada"});
                if(avaliacoes.Count() < 1) return NotFound("Nenhuma avaliação cadastrada.");

                return Ok(avaliacoes);
            }
            catch(Exception ex)
            {
                return BadRequest("Erro ao lista dados. " + ex.Message);
            }
            
        }

        /// <summary>
        /// busca as avaliações com o Id do usuario passado.
        /// </summary>
        /// <remarks>
        /// Exemplo de Retorno:
        ///
        ///     GET http://brunohafonso-001-site1.ctempurl.com/api/Avaliacao/buscarid/{Id}
        ///    
         ///     {
        ///         "avTrajeto": 0,
        ///         "avSeguranca": 0,
        ///         "rotaRealizada": {
        ///             "latInicio": 0,
        ///             "lngInicio": 0,
        ///             "latFim": 0,
        ///             "lngFim": 0,
        ///             "duracaoString": "string",
        ///             "duracaoInt": 0,
        ///             "kilometros": 0,
        ///             "login": null,
        ///             "idLogin": 0,
        ///             "rotaPesquisada": null,
        ///             "idRotaPesquisada": 1,
        ///             "id": 0,
        ///             "atualizadoEm": "0001-01-01T00:00:00",
        ///             "atualizadoPor": null,
        ///             "criadoEm": "2018-03-25T16:11:23.9320365",
        ///             "qtdAtualizacoes": 0
        ///         },
        ///         "idRotaRealizada": 0,
        ///         "login": null,
        ///         "idLogin": 0,
        ///         "id": 0,
        ///         "criadoEm": "2018-03-25T16:11:23.9330105",
        ///         "atualizadoEm": "0001-01-01T00:00:00",
        ///         "qtdAtualizacoes": 0,
        ///         "atualizadoPor": null
        ///     }
        /// 
        /// </remarks>
        /// <param name="Id">Id do login onde as avaliações serão buscadas.</param>
        /// <returns>lista de avaliações com o Id pesquisado.</returns>
        /// <response code="200"> Retorna lista com todas as avaliações realizadas com base no Id pesquisado.</response>
        /// <response code="404"> Nenhuma avaliação cadastrada com esse perfil pesquisado.</response>
        [Route("buscarid/{Id}")]
        [HttpGet]
        [EnableCors("AllowAnyOrigin")]
        [Authorize("Bearer")]
        public IActionResult BuscarPorId(int Id)
        {
                var avaliacoes = _avaliacaoRepository.Listar(new string[]{"RotaRealizada"}).Where(c => c.IdLogin == Id);
                if(avaliacoes.Count() > 0) 
                    return Ok(avaliacoes);
                else
                    return NotFound("Não existe nenhuma avaliação com  Id deste usuário.");                
        }

        /// <summary>
        /// Efetua o cadastro de avaliações.
        /// </summary>
        /// <remarks>
        /// Exemplo de Requisição:
        ///
        ///     POST http://brunohafonso-001-site1.ctempurl.com/api/Avaliacao/cadastrar
        ///    
        ///     {
        ///         "avTrajeto": 0,
        ///         "avSeguranca": 0,
        ///         "idRotaRealizada": 0,
        ///         "idLogin": 0       
        ///     }
        /// 
        /// </remarks>
        /// <param name="avaliacao">Dados da avaliação conforme criterios estabelecidos (precisa receber o objeto inteiro).</param>
        /// <returns>String informando se o objeto foi cadastrado.</returns>
        /// <response code="200"> Retorna string informando que a avaliação foi cadastrada com sucesso.</response>
        /// <response code="400"> Ocorreu um erro.</response>
        [Route("cadastrar")]
        [HttpPost]
        [EnableCors("AllowAnyOrigin")]
        [Authorize("Bearer")]
        public IActionResult Cadastrar([FromBody] Avaliacao avaliacao)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            try 
            {
                avaliacao.CriadoEm = DateTime.Now;
                avaliacao.QtdAtualizacoes = 0;
                avaliacao.AtualizadoPor = null;
                
                 _avaliacaoRepository.Inserir(avaliacao);
                return Ok("Avaliação salva com sucesso.");
            }
            catch(Exception ex)
            {
                return BadRequest("Erro ao cadastrar avaliação de rota. " + ex.Message);
            }
        }
        
        /// <summary>
        /// Efetua a atualização dos dados da avaliação da rota.
        /// </summary>
        /// <remarks>
        /// Exemplo de Requisição:
        ///
        ///     PUT http://brunohafonso-001-site1.ctempurl.com/api/Avaliacao/atualizar
        ///     
        ///     {
        ///         "avTrajeto": 0,
        ///         "avSeguranca": 0,
        ///         "idRotaRealizada": 0,
        ///         "idLogin": 0,
        ///         "qtdAtualizacoes": 0
        ///     }
        /// 
        /// </remarks>
        /// <param name="avaliacao">Dados da avaliação conforme criterios estabelecidos (precisa receber o objeto inteiro).</param>
        /// <returns>String informando se a avaliação foi atualizada com sucesso.</returns>
        /// <response code="200"> Retorna string informando que a avaliação foi atualizada com sucesso.</response>
        /// <response code="400"> Ocorreu um erro.</response>
        [Route("atualizar")]
        [HttpPut]
        [EnableCors("AllowAnyOrigin")]
        [Authorize("Bearer")]
        public IActionResult Atualizar([FromBody] Avaliacao avaliacao)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                avaliacao.AtualizadoEm = DateTime.Now;
                avaliacao.QtdAtualizacoes = avaliacao.QtdAtualizacoes + 1;
                avaliacao.AtualizadoPor = _loginRepository.Listar().FirstOrDefault(c => c.Id == avaliacao.IdLogin).NomeUsuario;

                _avaliacaoRepository.Atualizar(avaliacao);
                return Ok("Avaliação atualizada com sucesso. ");
            }
            catch(Exception ex)
            {
                return BadRequest("Erro ao atualizar avaliação. " + ex.Message);
            }
        }

        /// <summary>
        /// Busca os dados necessários para gerar gráficos das condições do sistema cicloviário de acordo com as avaliações dos usuários.
        /// </summary>
        /// <remarks>
        /// Exemplo de Retorno:
        /// 
        ///     GET http://brunohafonso-001-site1.ctempurl.com/api/Avaliacao/graficos
        /// 
        ///     {
        ///         "avaliacoesRuinsCiclovias": 0,
        ///         "avaliacoesRestoCiclovias": 0,
        ///         "labelRuimCiclovias": "string",
        ///         "labelRestoCiclovias": "string",
        ///         "avaliacoesRuinsSeguranca": 0,
        ///         "avaliacoesRestoSeguranca": 0,
        ///         "labelRuimSeguranca": "string",
        ///         "labelRestoSeguranca": "string",
        ///         "totalAvaliacoes": 0
        ///     }
        /// 
        /// </remarks>
        /// <returns>Dados necessários para gerar gráficos das condições do sistema cicloviário de acordo com as avaliações dos usuários.</returns>
        /// <response code="200"> Retorna objeto com dados sobre as avaliações dos usuários.</response>
        /// <response code="400"> Ocorreu um erro.</response>
        [Route("graficos")]
        [HttpGet]
        [EnableCors("AllowAnyOrigin")]
        [AllowAnonymous]
        public IActionResult Graficos()
        {
            try 
            {
                var avaliacoesRuinsCiclovias = _avaliacaoRepository.Listar().Where(a => a.AvTrajeto < 3).Count();
                var avaliacoesRestoCiclovias = _avaliacaoRepository.Listar().Where(a => a.AvTrajeto >= 3).Count();
                string labelRuimCiclovias = "Ruim ou Muito Ruim";
                string labelRestoCiclovias = "Outras Opiniões";

                var avaliacoesRuinsSeguranca = _avaliacaoRepository.Listar().Where(a => a.AvTrajeto < 3).Count();
                var avaliacoesRestoSeguranca = _avaliacaoRepository.Listar().Where(a => a.AvTrajeto >= 3).Count();
                string labelRuimSeguranca = "Ruim ou Muito Ruim";
                string labelRestoSeguranca = "Outras Opiniões";
                
                var totalAvaliacoes = _avaliacaoRepository.Listar().Count();

                var retorno = new {
                    avaliacoesRuinsCiclovias = avaliacoesRuinsCiclovias,
                    avaliacoesRestoCiclovias = avaliacoesRestoCiclovias,
                    labelRuimCiclovias = labelRuimCiclovias,
                    labelRestoCiclovias = labelRestoCiclovias,
                    avaliacoesRuinsSeguranca = avaliacoesRuinsSeguranca,
                    avaliacoesRestoSeguranca = avaliacoesRestoSeguranca,
                    labelRuimSeguranca = labelRuimSeguranca,
                    labelRestoSeguranca = labelRestoSeguranca,
                    totalAvaliacoes = totalAvaliacoes
                };

                return Ok(retorno);
            }
            catch(Exception ex)
            {
                return BadRequest("Erro ao buscar dados no banco. " + ex.Message);
            }
        }
    }
}