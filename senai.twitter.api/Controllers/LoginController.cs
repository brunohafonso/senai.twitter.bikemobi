using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using senai.twitter.domain.Contracts;
using senai.twitter.domain.Entities;
using senai.twitter.repository.Repositories;

namespace senai.twitter.api.Controllers
{
    [Route("api/cadastro")]
    public class LoginController : Controller
    {
        private IBaseRepository<Login> _loginRepository;
        private IBaseRepository<Perfil> _perfilRepository;
        private IBaseRepository<RotaPesquisa> _rotaPesquisaRepository;

        public LoginController(IBaseRepository<Login> loginRepository, IBaseRepository<Perfil> perfilRepository)
        {
            _loginRepository = loginRepository;
            _perfilRepository = perfilRepository;
        }

        public static string EncriptarSenha(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }        

        [Route("login")]
        [HttpPost]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult Validar([FromBody] Login login, [FromServices] SigningConfigurations signingConfigurations, [FromServices] TokenConfigurations tokenConfigurations)
        {
            Login log = _loginRepository.Listar().FirstOrDefault(c => c.Email == login.Email || c.NomeUsuario == login.NomeUsuario && c.Senha == EncriptarSenha(login.Senha));
            if (log != null)
            {
                ClaimsIdentity identity = new ClaimsIdentity(
                    new GenericIdentity(log.Id.ToString(), "Login"),
                    new[] {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                        new Claim(JwtRegisteredClaimNames.UniqueName, log.Id.ToString()),
                        new Claim("Nome", log.NomeUsuario),
                        new Claim(ClaimTypes.Email, log.Email)
                    }
                );

                DateTime dataCriacao = DateTime.Now;
                DateTime dataExpiracao = dataCriacao + TimeSpan.FromSeconds(tokenConfigurations.Seconds);

                var handler = new JwtSecurityTokenHandler();
                var securityToken = handler.CreateToken(new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
                {
                    Issuer = tokenConfigurations.Issuer,
                    Audience = tokenConfigurations.Audience,
                    SigningCredentials = signingConfigurations.SigningCredentials,
                    Subject = identity
                });

                var token = handler.WriteToken(securityToken);
                var retorno = new
                {
                    autenticacao = true,
                    created = dataCriacao.ToString("yyyy-MM-dd HH:mm:ss"),
                    expiration = dataExpiracao.ToString("yyyy-MM-dd HH:mm:ss"),
                    acessToken = token,
                    message = "Ok"
                };

                return Ok(retorno);
            }

            var retornoerro = new
            {
                autenticacao = false,
                message = "Falha na Autenticação"
            };

            return BadRequest(retornoerro);
            
        }
        
        
        /// <summary>
        /// Busca todos os logins na base de dados
        /// </summary>
        /// <returns>Lista com todos os logins na base de dados</returns>
        [Route("todos")]
        [HttpGet]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult Buscar()
        {
            var logins = _loginRepository.Listar(new string[]{"Perfil","RotasPesquisas"});
            return Ok(logins);
        }

        /// <summary>
        /// busca um login com o Id passado
        /// </summary>
        /// <param name="id">Id do login a ser buscado</param>
        /// <returns>Objeto login com o Id pesquisado</returns>
        [Route("burcarid/{id}")]
        [HttpGet]
        [EnableCors("AllowAnyOrigin")]
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
        [EnableCors("AllowAnyOrigin")]
        public IActionResult Cadastrar([FromBody] Login login)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                login.Senha = EncriptarSenha(login.Senha);
                login.CriadoEm = DateTime.Now;
                login.QtdAtualizacoes = 0;
                login.AtualizadoPor = null;

                login.Perfil.CriadoEm = DateTime.Now;
                login.Perfil.QtdAtualizacoes = 0;
                login.Perfil.AtualizadoPor = null;

                _loginRepository.Inserir(login);
                return Ok($"Usuário {login.NomeUsuario} Cadastrado Com Sucesso.");
            }
            catch (Exception ex)
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
        [EnableCors("AllowAnyOrigin")]
        public IActionResult Atualizar([FromBody] Login login)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                login.Senha = EncriptarSenha(login.Senha);
                login.AtualizadoEm = DateTime.Now;
                login.QtdAtualizacoes = login.QtdAtualizacoes + 1;
                login.AtualizadoPor = login.NomeUsuario;

                _loginRepository.Atualizar(login);
                return Ok($"Usuário {login.NomeUsuario} Atualizado Com Sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest("Erro ao atualizar dados. " + ex.Message);
            }
        }


        // [Route("deletar")]
        // [HttpDelete]
        // public IActionResult Deletar([FromBody] Login login)
        // {
        //     if(!ModelState.IsValid)
        //         return BadRequest(ModelState);

        //     try
        //     {
        //          _loginRepository.Deletar(login);
        //          return Ok($"Usuário {login.NomeUsuario} Deletado Com Sucesso.");
        //     }
        //     catch(Exception ex)
        //     {
        //         return BadRequest("Erro ao deletar dados. " + ex.Message);
        //     }
        // }
    }
}