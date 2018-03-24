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
using SendGrid;
using SendGrid.Helpers.Mail;

namespace senai.twitter.api.Controllers
{
    [Route("api/cadastro")]
    public class LoginController : Controller
    {
        
        public string Dominio = "localhost:5000/";
        private readonly TokenConfigurations _tokenOptions;
        private IBaseRepository<Login> _loginRepository;
        private IBaseRepository<Perfil> _perfilRepository;
        private IBaseRepository<RotaPesquisada> _rotaPesquisadaRepository;

        private IBaseRepository<RotaRealizada> _rotaRealizadaRepository;

        private IBaseRepository<Avaliacao> _avaliacaoRepository;

        private IBaseRepository<RequisicaoAlterarSenha> _requisicaoAlterarSenhaRepository;

        public LoginController(IBaseRepository<Login> loginRepository,IBaseRepository<Perfil> perfilRepository, IBaseRepository<RotaPesquisada> rotaPesquisadaRepository,IBaseRepository<RotaRealizada> rotaRealizadaRepository, IBaseRepository<Avaliacao> avaliacaoRepository, TokenConfigurations tokenOptions, IBaseRepository<RequisicaoAlterarSenha> requisicaoAlterarSenhaRepository)
        {
            _loginRepository = loginRepository;
            _perfilRepository = perfilRepository;
            _rotaPesquisadaRepository = rotaPesquisadaRepository;
            _rotaRealizadaRepository = rotaRealizadaRepository;
            _avaliacaoRepository = avaliacaoRepository;
            _requisicaoAlterarSenhaRepository = requisicaoAlterarSenhaRepository;
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
        
        public static void EnviarEmail(Login login, string assunto, string mensagem, string htmlmensagem)
        {
            var client = new SendGridClient("");
            var from = new EmailAddress("brunohafonso@gmail.com", "BikeMobi Support");
            var subject = assunto;
            var to = new EmailAddress(login.Email, login.NomeUsuario);
            var plainTextContent = mensagem;
            var htmlContent = htmlmensagem;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = client.SendEmailAsync(msg);
        }      

        /// <summary>
        /// Efetua login
        /// </summary>
        /// <param name="login">Email ou Nome do usuário e Senha do usuário.</param>
        /// <returns>Dados do token caso a autenticação tenha dado sucesso.</returns>
        [Route("login")]
        [HttpPost]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult Logar([FromBody] Login login, [FromServices] SigningConfigurations signingConfigurations, [FromServices] TokenConfigurations tokenConfigurations)
        {
            Login log = _loginRepository.Listar().FirstOrDefault(c => (c.Email == login.Email || c.NomeUsuario == login.NomeUsuario) && c.Senha == EncriptarSenha(login.Senha));
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
                    Subject = identity,
                    NotBefore = dataCriacao,
                    Expires = dataExpiracao
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
        /// Solicita alteração de senha
        /// </summary>
        /// <param name="login">Email e nome de usuário.</param>
        /// <returns>Se a solicitação foi finalizada com sucesso.</returns>
        [Route("esqueciminhasenha")]
        [HttpPost]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult SolicitarResetSenha([FromBody] Login login)
        {
            Login log = _loginRepository.Listar().FirstOrDefault(c => c.NomeUsuario == login.NomeUsuario && c.Email == login.Email);
            if(log != null) 
                try 
                {
                    var requisicao = new RequisicaoAlterarSenha(log.Id, log.Email);
                    _requisicaoAlterarSenhaRepository.Inserir(requisicao);
                    var requisicoesAntigas = _requisicaoAlterarSenhaRepository.Listar().Where(r => r.IdLogin == log.Id && r.Id != requisicao.Id);

                    if(requisicoesAntigas != null)
                        foreach(var item in requisicoesAntigas)
                        {
                            if(item.Status == "Ativo")
                            {
                                item.Status = "Expirado";
                                item.AtualizadoEm = DateTime.Now;
                                item.AtualizadoPor = log.NomeUsuario;
                                item.QtdAtualizacoes = item.QtdAtualizacoes + 1; 
                                _requisicaoAlterarSenhaRepository.Atualizar(item);
                            }
                        }


                    string link = Dominio + $"'api/cadastro/resetarsenha/{requisicao.Id}'";

                    string mensagem = $"Olá, {log.NomeUsuario}, Pronto para escolher sua senha? \n Refefinir Senha \n (Lembre-se: Você tem 30 minutos para escolher a senha. Após esse período, será necessário solicitar outra redefinição de senha.)";
                    string htmlmensagem = $"<div style='font-family: Helvetica,Arial,sans-serif; max-width: 650px; margin: auto;'><img width='212px' height='72px' src='http://bike-mobi.herokuapp.com/static/media/logoBikeMobi.9f5f6ad8.png'/><h1>Pronto para escolher sua senha?</h1> <a style='cursor: pointer; text-align: center; border-radius: 4px; background-color: #5db8fc; text-decoration: none; font-size: 25px; font-weight: bold; color: #fff; width: 400px; margin: auto; padding: 5px 10px' href={link}>Redefinir Senha</a><h2>(Lembre-se: Você tem 30 minutos para escolher a senha. Após esse período, será necessário solicitar outra redefinição de senha.)</h2></div>";

                    EnviarEmail(log, "Redefina sua senha do BikeMobi", mensagem, htmlmensagem);
                    return Ok("Sua alteração de senha foi recebida com sucesso, em breve você receberá um email com as instruções para alteração da senha.");
                }
                catch(Exception ex)
                {
                    return BadRequest("Erro ao solicitar nova senha. " + ex.Message);
                }
                   
            return BadRequest("Nome de usuário e/ou email não cadastrados.");
        }

        /// <summary>
        /// Finaliza a alteração da senha.
        /// </summary>
        /// <param name="login">Senha do usuário.</param>
        /// <param name="Id">Id da requisição de alteração de senha.</param>
        /// <returns>Se a alteração de senha foi finalizada com sucesso.</returns>
        [Route("resetarsenha/{Id}")]
        [HttpPost]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult ResetarSenha(int Id, [FromBody] Login login)
        {
            var requisicao = _requisicaoAlterarSenhaRepository.BuscarPorId(Id);
            if(requisicao == null) return BadRequest("Requisição de alteração de senha inexistente.");

            var expiracao = requisicao.Expiracao;
            var expirada = expiracao.CompareTo(DateTime.Now);
            if(expirada <= 0) 
                try
                {
                    if(requisicao.Status == "Ativo")
                    {
                        requisicao.Status = "Expirado";
                        requisicao.AtualizadoEm = DateTime.Now;
                        requisicao.AtualizadoPor = _loginRepository.BuscarPorId(requisicao.IdLogin).NomeUsuario;
                        requisicao.QtdAtualizacoes = requisicao.QtdAtualizacoes + 1; 
                        _requisicaoAlterarSenhaRepository.Atualizar(requisicao);
                        return Ok("Requisição de alteração de senha expirada.");
                    }
                    else
                    {
                        return Ok("Requisição de alteração de senha expirada.");
                    }
                }
                catch(Exception ex)
                {
                    return BadRequest("Erro ao processar alteração de senha. "+ ex.Message);
                }
            
            Login log = _loginRepository.BuscarPorId(requisicao.IdLogin);
            log.Senha = EncriptarSenha(login.Senha);
            log.AtualizadoPor = log.NomeUsuario;
            log.AtualizadoEm = DateTime.Now;
            log.QtdAtualizacoes = log.QtdAtualizacoes + 1;

            try 
            {
                requisicao.Status = "Realizada";
                requisicao.AtualizadoEm = DateTime.Now;
                requisicao.AtualizadoPor = _loginRepository.BuscarPorId(requisicao.IdLogin).NomeUsuario;
                requisicao.QtdAtualizacoes = requisicao.QtdAtualizacoes + 1; 
                _requisicaoAlterarSenhaRepository.Atualizar(requisicao);
                _loginRepository.Atualizar(log);
                return Ok("Senha alterada com sucesso.");
            }
            catch(Exception ex)
            {
                return BadRequest("Erro ao finalizar a alteração de senha. " + ex.Message);
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

                long tempoTrajetos = 0;

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