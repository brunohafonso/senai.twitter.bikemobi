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
        /// <returns>Lista todas as avaliações realizadas.</returns>
        [Route("todos")]
        [HttpGet]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult Buscar()
        {
            try
            {
                var avaliacoes = _avaliacaoRepository.Listar(new string[]{"RotaRealizada"});
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
        /// <param name="Id">Id do login onde as avaliações serão buscadas.</param>
        /// <returns>lista de avaliações com o Id pesquisado.</returns>
        [Route("buscarid/{Id}")]
        [HttpGet]
        [EnableCors("AllowAnyOrigin")]
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
        /// <param name="avaliacao">Dados da avaliação conforme criterios estabelecidos (precisa receber o objeto inteiro).</param>
        /// <returns>String informando se o objeto foi cadastrado.</returns>
        [Route("cadastrar")]
        [HttpPost]
        [EnableCors("AllowAnyOrigin")]
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
        /// <param name="avaliacao">Dados da avaliação conforme criterios estabelecidos (precisa receber o objeto inteiro).</param>
        /// <returns>String informando se a avaliação foi atualizada com sucesso.</returns>
        [Route("atualizar")]
        [HttpPut]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult Atualizar([FromBody] Avaliacao avaliacao)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                avaliacao.AtualizadoEm = DateTime.Now;
                avaliacao.QtdAtualizacoes = avaliacao.QtdAtualizacoes + 1;

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
        /// <returns>Dados necessários para gerar gráficos das condições do sistema cicloviário de acordo com as avaliações dos usuários.</returns>
        [Route("graficos")]
        [HttpGet]
        [AllowAnonymous]
        [EnableCors("AllowAnyOrigin")]
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