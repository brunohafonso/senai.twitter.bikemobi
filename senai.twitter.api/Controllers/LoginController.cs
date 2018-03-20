using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using senai.twitter.domain.Contracts;
using senai.twitter.domain.Entities;
using senai.twitter.repository.Repositories;

namespace senai.twitter.api.Controllers
{
    [Route("api/cadastro")]
    public class LoginController : Controller
    {
        
        private readonly TokenConfigurations _tokenOptions;
        private IBaseRepository<Login> _loginRepository;
        private IBaseRepository<Perfil> _perfilRepository;
        private IBaseRepository<RotaPesquisada> _rotaPesquisadaRepository;

        private IBaseRepository<RotaRealizada> _rotaRealizadaRepository;

        private IBaseRepository<Avaliacao> _avaliacaoRepository;

        public LoginController(IBaseRepository<Login> loginRepository,IBaseRepository<Perfil> perfilRepository, IBaseRepository<RotaPesquisada> rotaPesquisadaRepository,IBaseRepository<RotaRealizada> rotaRealizadaRepository, IBaseRepository<Avaliacao> avaliacaoRepository, TokenConfigurations tokenOptions)
        {
            _loginRepository = loginRepository;
            _perfilRepository = perfilRepository;
            _rotaPesquisadaRepository = rotaPesquisadaRepository;
            _rotaRealizadaRepository = rotaRealizadaRepository;
            _avaliacaoRepository = avaliacaoRepository;
            _tokenOptions = tokenOptions;
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

        /// <summary>
        /// Efetua login
        /// </summary>
        /// <param name="login">Email ou nome de Usuario e senha</param>
        /// <returns>Dados do token caso a autenticação tenha dado sucesso.</returns>
        [Route("login")]
        [HttpPost]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult Validar([FromBody] Login login, [FromServices] SigningConfigurations signingConfigurations, [FromServices] TokenConfigurations tokenConfigurations)
        {
            Login log = _loginRepository.Listar().FirstOrDefault(c => c.Email == login.Email && c.Senha == EncriptarSenha(login.Senha));
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
        
        [HttpPost]
        [Route("teste")]
        public IActionResult teste([FromBody] string token)
        {
            var validationParameters = new TokenValidationParameters()
            {
                ValidIssuer = _tokenOptions.Issuer,
                ValidAudience = _tokenOptions.Audience,
                IssuerSigningKey = _tokenOptions.SigningKey,
                RequireExpirationTime = true
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = null;

            try
            {
                return Ok(tokenHandler.ValidateToken(token, validationParameters, out securityToken));
            }
            catch(Exception ex)
            {
                return BadRequest("Token Invalido. " + ex.Message);
            }


        }


        /// <summary>
        /// Retorna dados do historico do usuário
        /// </summary>
        /// <param name="Id">Id do usuári que o histórico será pesquisado.</param>
        /// <returns>Dados do Histórico do usuário.</returns>
        [Route("historico/{Id}")]
        [HttpGet]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult Historico(int Id)
        {
            var usuario =  _loginRepository.BuscarPorId(Id, new string[]{"Perfil"});

            if(usuario != null) {
                var rotasPesquisadas = _rotaPesquisadaRepository.Listar().Where(c => c.IdLogin == Id);

                var rotasRealizadas = _rotaRealizadaRepository.Listar().Where(c => c.IdLogin == Id);

                var avaliacoes = _avaliacaoRepository.Listar().Where(c => c.IdLogin == Id);

                var tempoTrajetos = 0;

                foreach(var item in rotasRealizadas)
                {
                    tempoTrajetos = tempoTrajetos + item.DuracaoInt;
                }
                
                var retorno = new {
                    QtdAtualizacoesLogin = usuario.QtdAtualizacoes,
                    ultimaAtualizacaoLogin = usuario.AtualizadoEm,
                    QtdAtualizacoesPerfil = usuario.Perfil.QtdAtualizacoes,
                    ultimaAtualizacaoPerfil = usuario.Perfil.AtualizadoEm,
                    rotaPesquisadas = rotasPesquisadas.Count(),
                    dataUltimaPesquisa = rotasPesquisadas.Last().CriadoEm,
                    QtdRotasRealizadas = rotasRealizadas.Count(),
                    dataUltimaRotaRealizada = rotasRealizadas.Last().CriadoEm,
                    QtdAvaliacoes = avaliacoes.Count(),
                    dataUltimaAvaliacao = avaliacoes.Last().CriadoEm,
                    tempoUsoApp = (tempoTrajetos / 60) + " mins"
                };

                return Ok(retorno);
            }
            else 
            {
                return BadRequest("Não existe nenhum usuario com esse Id cadastrado. ");
            }
                
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
            try {
                var logins = _loginRepository.Listar(new string[]{"Perfil"});
                return Ok(logins);
            } 
            catch(Exception ex)
            {
                return BadRequest("Erro ao buscar dados. " + ex.Message);
            }
            
        }

        /// <summary>
        /// busca um login com o Id passado
        /// </summary>
        /// <param name="Id">Id do login a ser buscado</param>
        /// <returns>Objeto login com o Id pesquisado</returns>
        [Route("buscarid/{Id}")]
        [HttpGet]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult BuscarPorId(int Id)
        {
            var login = _loginRepository.BuscarPorId(Id, new string[]{"Perfil"});
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