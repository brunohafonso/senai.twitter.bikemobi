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
        private IBaseRepository<RotaPesquisa> _rotaPesquisaRepository;

        public LoginController(IBaseRepository<Login> loginRepository,IBaseRepository<Perfil> perfilRepository)
        {

            _loginRepository = loginRepository;
            _perfilRepository = perfilRepository;
        }

        [HttpGet]
        public IActionResult Buscar()
        {
            var logins = _loginRepository.Listar(new string[]{"RotasPesquisas"});
            return Ok(logins);
        }

        [HttpGet("{id}")]
        public IActionResult BuscarPorId(int id)
        {
            var login = _loginRepository.BuscarPorId(id);
            var perfil = _perfilRepository.BuscarPorId(id);
            login.Perfil = perfil;
            if (login != null)
                return Ok(login);
            else
                return NotFound();
        }


        /// <summary>
        /// Efetua o cadastro de Logins juntamente com os dados básicos do perfil
        /// </summary>
        /// <param name="login">Dados do login/perfil conforme criterios estabelecidos (precisa receber o objeto inteiro)</param>
        /// <returns>String informando qual objeto foi cadastrado.</returns>
        [Route("cadastrar")]
        [HttpPost]
        public IActionResult Cadastrar([FromBody] Login login)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            try 
            {
                login.CriadoEm = DateTime.Now;
                login.QtdAtualizacoes = 0;
                login.AtualizadoPor = null;
                
                login.Perfil.CriadoEm = DateTime.Now;
                login.Perfil.QtdAtualizacoes = 0;
                login.Perfil.AtualizadoPor = null;

                _loginRepository.Inserir(login);
                return Ok($"Usuário {login.NomeUsuario} Cadastrado Com Sucesso.");
            }
            catch(Exception ex)
            {
                return BadRequest("Erro ao cadastrar dados. " + ex.Message);
            }
            
        }
        
        /// <summary>
        /// Efetua a atualização dos dados do Login juntamente com os dados básicos do perfil
        /// </summary>
        /// <param name="login">Dados do login/perfil conforme criterios estabelecidos (precisa receber o objeto inteiro)</param>
        /// <returns>String informando qual objeto foi atualizado.</returns>
        [Route("atualizar")]
        [HttpPut]
        public IActionResult Atualizar([FromBody] Login login)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                login.AtualizadoEm = DateTime.Now;
                login.QtdAtualizacoes = login.QtdAtualizacoes + 1;
                login.AtualizadoPor = login.NomeUsuario;
                    
                _loginRepository.Atualizar(login);
                return Ok($"Usuário {login.NomeUsuario} Atualizado Com Sucesso.");
            }
            catch(Exception ex)
            {
                return BadRequest("Erro ao atualizar dados. " + ex.Message);
            }
        }

        /// <summary>
        /// Efetua a atualização dos dados do Login juntamente com os dados básicos do perfil
        /// </summary>
        /// <param name="login">Dados do login/perfil conforme criterios estabelecidos (precisa receber o objeto inteiro)</param>
        /// <returns>String informando qual objeto foi deletado.</returns>
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