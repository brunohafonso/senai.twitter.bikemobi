using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Authorization;
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
        
        public string Dominio = "http://brunohafonso-001-site1.ctempurl.com/";
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
        /// Efetua login.
        /// </summary>
        /// <remarks>
        /// Exemplo de Requisição:
        ///
        ///     POST http://brunohafonso-001-site1.ctempurl.com/api/cadastro/login
        ///    
        ///     {
        ///         "nomeUsuario": "string",
        ///         "senha": "string"
        ///     }
        /// 
        ///     OU
        /// 
        ///     {
        ///         "email": "string",
        ///         "senha": "string"
        ///     }
        ///
        /// </remarks>
        /// <param name="login">Email ou Nome do usuário e Senha do usuário.</param>
        /// <returns>Dados do token caso a autenticação tenha dado sucesso.</returns>
        /// <response code="200">Dados do token caso a autenticação tenha dado sucesso.</response>
        /// <response code="400">Falha na autenticação.</response>
        [Route("login")]
        [HttpPost]
        [EnableCors("AllowAnyOrigin")]
        [AllowAnonymous]
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
                message = "Falha na Autenticação: Email e/ou senha incorretos."
            };

            return BadRequest(retornoerro);
            
        }

        /// <summary>
        /// Solicita alteração de senha
        /// </summary>
        /// <remarks>
        /// Exemplo de Requisição:
        ///
        ///     POST http://brunohafonso-001-site1.ctempurl.com/api/cadastro/esqueciminhasenha
        ///    
        ///     {
        ///         "email": "string",
        ///         "nomeUsuario": "string"
        ///     }
        /// 
        /// </remarks>
        /// <param name="login">Email e nome de usuário.</param>
        /// <returns>Se a solicitação foi finalizada com sucesso.</returns>
        /// <response code="200"> Retorna uma mensagem informando que a requisição de senha foi finalizada com sucesso.</response>
        /// <response code="400"> Ocorreu um erro ao solicitar redefinição de senha.</response>
        /// <response code="404">Usuário e/ou email nao cadastrados.</response>
        [Route("esqueciminhasenha")]
        [HttpPost]
        [EnableCors("AllowAnyOrigin")]
        [AllowAnonymous]
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


                    string link = Dominio + $"api/cadastro/resetarsenha/{requisicao.Id}";

                    string mensagem = $"Olá, {log.NomeUsuario}, Pronto para escolher sua senha? \n Refefinir Senha \n (Lembre-se: Você tem 30 minutos para escolher a senha. Após esse período, será necessário solicitar outra redefinição de senha.)";
                    string htmlmensagem = $@"<div style='font-family: Helvetica,Arial,sans-serif; max-width: 650px; margin: auto;'><img width='212px' height='72px' src='http://bike-mobi.herokuapp.com/static/media/logoBikeMobi.9f5f6ad8.png'/><h1>Olá, {log.NomeUsuario}, Pronto para escolher sua senha?</h1> <a style='cursor: pointer; text-align: center; border-radius: 4px; background-color: #5db8fc; text-decoration: none; font-size: 25px; font-weight: bold; color: #fff; width: 400px; margin: auto; padding: 5px 10px' href={link}>Redefinir Senha</a><h2>(Lembre-se: Você tem 30 minutos para escolher a senha. Após esse período, será necessário solicitar outra redefinição de senha.)</h2></div>";

                    EnviarEmail(log, "Redefina sua senha do BikeMobi", mensagem, htmlmensagem);
                    return Ok("Sua alteração de senha foi recebida com sucesso, em breve você receberá um email com as instruções para alteração da senha.");
                }
                catch(Exception ex)
                {
                    return BadRequest("Erro ao solicitar nova senha. " + ex.Message);
                }
                   
            return NotFound("Nome de usuário e/ou email não cadastrados.");
        }

        /// <summary>
        /// Finaliza a alteração da senha.
        /// </summary>
        /// <remarks>
        /// Exemplo de Requisição:
        ///
        ///     POST http://brunohafonso-001-site1.ctempurl.com/api/cadastro/resetarsenha/{Id}
        ///    
        ///     {
        ///         "senha": "string"
        ///     }
        /// 
        /// </remarks>
        /// <param name="login">Senha do usuário.</param>
        /// <param name="Id">Id da requisição de alteração de senha.</param>
        /// <returns>Se a alteração de senha foi finalizada com sucesso.</returns>
        /// <response code="200"> Retorna que senha foi alterada com sucesso.</response>
        /// <response code="400"> Ocorreu um erro</response>
        /// <response code="404"> Requisição de alteração senha não encontrada.</response>
        [Route("resetarsenha/{Id}")]
        [HttpPost]
        [EnableCors("AllowAnyOrigin")]
        [AllowAnonymous]
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
            
            if(requisicao.Status != "Expirado" && requisicao.Status != "Realizada") 
            {
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
            else
            {
                return BadRequest("Alteração de senha invalida ou expirada.");
            }
        }

        /// <summary>
        /// Retorna dados do historico do usuário.
        /// </summary>
        /// <remarks>
        /// Exemplo de Retorno:
        ///
        ///     GET http://brunohafonso-001-site1.ctempurl.com/api/cadastro/historico/{Id}
        ///    
        ///     {
        ///         "qtdAtualizacoesLogin": 0,
        ///         "ultimaAtualizacaoLogin": "2018-03-26T17:29:48.5162586",
        ///         "qtdAtualizacoesPerfil": 0,
        ///         "ultimaAtualizacaoPerfil": "2018-03-26T17:52:16.0957272",
        ///         "rotaPesquisadas": 0,
        ///         "dataUltimaPesquisa": "26/03/2018 15:41:05",
        ///         "ultimaPesquisa": {
        ///             "id": 0,
        ///             "idLogin": 0,
        ///             "origemEnd": "string",
        ///             "origemLat": 0,
        ///             "origemLng": 0,
        ///             "destinoEnd": "string",
        ///             "destinoLat": 0,
        ///             "destinoLng": 0,
        ///             "polylinePoints": "string",
        ///             "duracao": "string",
        ///             "distancia": 0
        ///         },
        ///         "qtdRotasRealizadas": 0,
        ///         "dataUltimaRotaRealizada": "26/03/2018 15:41:06",
        ///         "ultimaRotaRealizada": {
        ///             "id": 0,
        ///             "idLogin": 0,
        ///             "idRotaPesquisada": 0,
        ///             "latInicio": 0,
        ///             "lngInicio": 0,
        ///             "latFim": 0,
        ///             "lngFim": 0,
        ///             "kilometros": 0,
        ///             "duracaoInt": 0,
        ///             "duracaoString": "string"
        ///         },
        ///         "qtdAvaliacoes": 0,
        ///         "dataUltimaAvaliacao": "26/03/2018 12:24:17",
        ///         "ultimaAvaliacao": {
        ///             "id": 0,
        ///             "idLogin": 0,
        ///             "idRotaRealizada": 0,
        ///             "avSeguranca": 0,
        ///             "avTrajeto": 0
        ///         },
        ///         "tempoUsoApp": "string"
        ///     }
        /// 
        /// </remarks>
        /// <param name="Id">Id do usuário que o histórico será pesquisado.</param>
        /// <returns>Dados do Histórico do usuário.</returns>
        /// <response code="200"> Retorna o histórico do usuário buscado pelo Id.</response>
        /// <response code="404"> Usuário não Encontrado.</response>
        [Route("historico/{Id}")]
        [HttpGet]
        [EnableCors("AllowAnyOrigin")]
        //[Authorize("Bearer")]
        public IActionResult Historico(int Id)
        {
            var usuario =  _loginRepository.BuscarPorId(Id, new string[]{"Perfil","RotasPesquisadas","RotasRealizadas","Avaliacoes"});

            if(usuario != null) {
                var rotasPesquisadas = usuario.RotasPesquisadas;
                var qtdRotasPesquisadas = 0;
                var dtUltimaPesquisa = "";
                object ultimaPesquisa = null;

                if(rotasPesquisadas.Count() < 1) 
                {
                    qtdRotasPesquisadas = 0;
                } 
                else 
                {
                    qtdRotasPesquisadas = rotasPesquisadas.Count();
                    dtUltimaPesquisa = rotasPesquisadas.Last().CriadoEm.ToString();
                    ultimaPesquisa = rotasPesquisadas.Select(x => new {x.Id, x.IdLogin, x.OrigemEnd, x.OrigemLat, x.OrigemLng, x.DestinoEnd, x.DestinoLat, x.DestinoLng, x.PolylinePoints, x.Duracao, x.Distancia}).Last();
                }

                var rotasRealizadas = usuario.RotasRealizadas;
                var QtdRotasRealizadas = 0;
                var dataUltimaRotaRealizada = "";
                object ultimaRotaRealizada = null;

                if(rotasRealizadas.Count() < 1) 
                {
                    QtdRotasRealizadas = 0;
                }
                else 
                {
                    QtdRotasRealizadas = rotasRealizadas.Count();
                    dataUltimaRotaRealizada = rotasRealizadas.Last().CriadoEm.ToString();
                    ultimaRotaRealizada = rotasRealizadas.Select(x => new {x.Id, x.IdLogin, x.IdRotaPesquisada, x.LatInicio, x.LngInicio, x.LatFim, x.LngFim, x.Kilometros, x.DuracaoInt, x.DuracaoString}).Last();
                }

                var avaliacoes = usuario.Avaliacoes;
                var QtdAvaliacoes = 0;
                var dataUltimaAvaliacao = "";
                object ultimaAvaliacao = null;

                if(avaliacoes.Count() < 1) 
                {
                   QtdAvaliacoes = 0; 
                }
                else
                {
                    QtdAvaliacoes = avaliacoes.Count();
                    dataUltimaAvaliacao = avaliacoes.Last().CriadoEm.ToString();
                    ultimaAvaliacao = avaliacoes.Select(x => new {x.Id, x.IdLogin, x.IdRotaRealizada, x.AvSeguranca, x.AvTrajeto}).Last();
                }

                string tempoUsoApp = "Nenhuma rota realizada utilizando o aplicativo.";
                double tempoTrajetos = 0;

                if(rotasRealizadas.Count() > 0) 
                {
                    foreach(var item in rotasRealizadas)
                    {
                        tempoTrajetos = tempoTrajetos + item.DuracaoInt;
                    }
                }
                
                double calc = Math.Floor(tempoTrajetos / 60 / 60);

                if(tempoTrajetos > 0)
                {
                    if((tempoTrajetos / 60 ) < 60) 
                    {
                        tempoUsoApp = $"{tempoTrajetos / 60} minutos.";
                    } 
                    else if((tempoTrajetos / 60 / 60) == 1)
                    {
                         tempoUsoApp = $"{tempoTrajetos / 60 / 60} hora.";                      
                    } 
                    else if((tempoTrajetos / 60 / 60) > 1)
                    {
                        if((tempoTrajetos / 60 / 60) - calc != 0 && calc == 1) 
                        {
                            var result = (tempoTrajetos / 60 / 60) - calc;
                            tempoUsoApp = $"{Math.Floor(tempoTrajetos / 60 / 60)} hora e {Convert.ToInt32(result * 60)} minutos.";
                        }
                        else
                        {
                            if((tempoTrajetos / 60 / 60) - calc != 0) 
                            {
                                var result = (tempoTrajetos / 60 / 60) - calc;
                                tempoUsoApp = $"{Math.Floor(tempoTrajetos / 60 / 60)} horas e {Convert.ToInt32(result * 60)} minutos.";
                            }
                            else 
                            {
                                tempoUsoApp = $"{tempoTrajetos / 60 / 60} horas.";
                            }
                        }
                    }
                }

                var retorno = new {
                    QtdAtualizacoesLogin = usuario.QtdAtualizacoes,
                    ultimaAtualizacaoLogin = usuario.AtualizadoEm,
                    QtdAtualizacoesPerfil = usuario.Perfil.QtdAtualizacoes,
                    ultimaAtualizacaoPerfil = usuario.Perfil.AtualizadoEm,
                    rotaPesquisadas = qtdRotasPesquisadas,
                    dataUltimaPesquisa = dtUltimaPesquisa,
                    ultimaPesquisa = ultimaPesquisa,
                    QtdRotasRealizadas = QtdRotasRealizadas,
                    dataUltimaRotaRealizada = dataUltimaRotaRealizada,
                    ultimaRotaRealizada = ultimaRotaRealizada,
                    QtdAvaliacoes = QtdAvaliacoes,
                    dataUltimaAvaliacao = dataUltimaAvaliacao,
                    ultimaAvaliacao = ultimaAvaliacao,
                    tempoUsoApp = tempoUsoApp
                };

                return Ok(retorno);
            }
            else 
            {
                return NotFound("Não existe nenhum usuario com esse Id cadastrado.");
            }
                
        }
        
        /// <summary>
        /// Busca todos os logins na base de dados.
        /// </summary>
        /// <remarks>
        /// Exemplo de Retorno:
        /// 
        ///     GET http://brunohafonso-001-site1.ctempurl.com/api/cadastro/todos
        /// 
        ///     {
        ///         "nomeUsuario": "string",
        ///         "email": "string",
        ///         "senha": "string",
        ///         "perfil": {
        ///         "nome": "string",
        ///         "dataNascimento": "2018-03-27T03:55:57.332Z",
        ///         "estado": "string",
        ///         "cidade": "string",
        ///         "bio": "string",
        ///         "avatarUrl": "string",
        ///         "idLogin": 0,
        ///         "id": 0,
        ///         "atualizadoEm": "2018-03-26T17:52:16.0957272",
        ///         "atualizadoPor": "string",
        ///         "criadoEm": "2018-03-14T00:00:00",
        ///         "qtdAtualizacoes": 0
        ///     },
        ///         "rotasPesquisadas": null,
        ///         "rotasRealizadas": null,
        ///         "avaliacoes": null,
        ///         "id": 0,
        ///         "atualizadoEm": "2018-03-26T17:29:48.5162586",
        ///         "atualizadoPor": "string",
        ///         "criadoEm": "0001-01-01T00:00:00",
        ///         "qtdAtualizacoes": 0
        ///     }
        /// 
        /// </remarks>
        /// <returns>Lista com todos os logins na base de dados.</returns>
        /// <response code="200"> Retorna lista com todos os logins na base de dados.</response>
        /// <response code="400"> Ocorreu um erro.</response>
        /// <response code="404"> Nenhum login cadastrado.</response>
        [Route("todos")]
        [HttpGet]
        [EnableCors("AllowAnyOrigin")]
        [Authorize("Bearer")]
        public IActionResult Buscar()
        {
            try {
                var logins = _loginRepository.Listar(new string[]{"Perfil"});
                if(logins.Count() < 1) return NotFound("Nenhum login cadastrado.");

                return Ok(logins);
            } 
            catch(Exception ex)
            {
                return BadRequest("Erro ao buscar dados. " + ex.Message);
            }
            
        }

        /// <summary>
        /// Busca um login com o Id passado.
        /// </summary>
        /// <remarks>
        /// Exemplo de Retorno:
        ///
        ///     GET http://brunohafonso-001-site1.ctempurl.com/api/cadastro/buscarid/{Id}
        ///    
        ///     {
        ///         "nomeUsuario": "string",
        ///         "email": "string",
        ///         "senha": "string",
        ///         "perfil": {
        ///         "nome": "string",
        ///         "dataNascimento": "2018-03-27T03:55:57.332Z",
        ///         "estado": "string",
        ///         "cidade": "string",
        ///         "bio": "string",
        ///         "avatarUrl": "string",
        ///         "idLogin": 0,
        ///         "id": 0,
        ///         "atualizadoEm": "2018-03-26T17:52:16.0957272",
        ///         "atualizadoPor": "string",
        ///         "criadoEm": "2018-03-14T00:00:00",
        ///         "qtdAtualizacoes": 0
        ///     },
        ///         "rotasPesquisadas": null,
        ///         "rotasRealizadas": null,
        ///         "avaliacoes": null,
        ///         "id": 0,
        ///         "atualizadoEm": "2018-03-26T17:29:48.5162586",
        ///         "atualizadoPor": "string",
        ///         "criadoEm": "0001-01-01T00:00:00",
        ///         "qtdAtualizacoes": 0
        ///     }
        /// 
        /// </remarks> 
        /// <param name="Id">Id do logihi\n a ser buscado.</param>
        /// <returns>Objeto login com o Id pesquisado.</returns>
        /// <response code="200"> Retorna login buscado através do Id.</response>
        /// <response code="404"> Nenhum login com Id buscado cadastrado.</response>
        [Route("buscarid/{Id}")]
        [HttpGet]
        [EnableCors("AllowAnyOrigin")]
        [Authorize("Bearer")]
        public IActionResult BuscarPorId(int Id)
        {
            var login = _loginRepository.BuscarPorId(Id, new string[]{"Perfil"});
            if (login != null)
                return Ok(login);
            else
                return NotFound("Login não encontrado.");
        }


        /// <summary>
        /// Efetua o cadastro de Logins juntamente com os dados básicos do perfil.
        /// </summary>
        /// <remarks>
        /// Exemplo de Requisição:
        ///
        ///     POST http://brunohafonso-001-site1.ctempurl.com/api/cadastro/cadastrar
        ///    
        ///     {
        ///         "nomeUsuario": "string",
        ///         "email": "string",
        ///         "senha": "string",
        ///         "perfil": {
        ///             "nome": "string",
        ///             "dataNascimento": "2018-03-27T03:55:57.332Z",
        ///             "estado": "string",
        ///             "cidade": "string",
        ///             "bio": "string",
        ///             "avatarUrl": "string",
        ///         }
        ///     }
        /// 
        /// </remarks>
        /// <param name="login">Dados do login/perfil conforme criterios estabelecidos (precisa receber o objeto inteiro).</param>
        /// <returns>Mensagem informando qual login foi cadastrado.</returns>
        /// <response code="200"> Retorna mensagem informando qual login foi cadastrado.</response>
        /// <response code="400"> Ocorreu um erro.</response>
        [Route("cadastrar")]
        [HttpPost]
        [EnableCors("AllowAnyOrigin")]
        [AllowAnonymous]
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
        /// Efetua a atualização dos dados do Login juntamente com os dados básicos do perfil.
        /// </summary>
        /// <remarks>
        /// Exemplo de Requisição:
        ///
        ///     PUT http://brunohafonso-001-site1.ctempurl.com/api/cadastro/atualizar
        ///    
        ///     {
        ///         "nomeUsuario": "string",
        ///         "email": "string",
        ///         "senha": "string",
        ///         "perfil": {
        ///             "nome": "string",
        ///             "dataNascimento": "1992-03-02T00:00:00",
        ///             "estado": "string",
        ///             "cidade": "string",
        ///             "bio": "string",
        ///             "avatarUrl": "string",
        ///             "idLogin": 0,
        ///             "id": 0,
        ///             "qtdAtualizacoes": 0
        ///         },
        ///         "id": 0,
        ///         "qtdAtualizacoes": 0
        ///     }
        /// 
        /// </remarks>
        /// <param name="login">Dados do login/perfil conforme criterios estabelecidos (precisa receber o objeto inteiro).</param>
        /// <returns>String informando qual objeto foi atualizado.</returns>
        /// <response code="200"> Retorna mensagem informando qual login foi atualizado.</response>
        /// <response code="400"> Ocorreu um erro.</response> 
        [Route("atualizar")]
        [HttpPut]
        [EnableCors("AllowAnyOrigin")]
        [Authorize("Bearer")]
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

                login.Perfil.AtualizadoEm = DateTime.Now;
                login.Perfil.QtdAtualizacoes = login.Perfil.QtdAtualizacoes + 1;
                login.Perfil.AtualizadoPor = login.NomeUsuario;

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