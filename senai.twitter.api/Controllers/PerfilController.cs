using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using senai.twitter.domain.Contracts;
using senai.twitter.domain.Entities;
using senai.twitter.repository.Context;

namespace senai.twitter.api.Controllers
{
    [Route("api/[controller]")]
    public class PerfilController : Controller
    {
        private IBaseRepository<Login> _loginRepository;
        private IBaseRepository<Perfil> _perfilRepository;

        public PerfilController(IBaseRepository<Login> loginRepository,IBaseRepository<Perfil> perfilRepository)
        {
            _loginRepository = loginRepository;
            _perfilRepository = perfilRepository;
        }

       
        /// <summary>
        /// Busca todos os registros de perfis cadastrados.
        /// </summary>
        /// <remarks>
        /// Exemplo de Retorno:
        /// 
        ///     GET http://brunohafonso-001-site1.ctempurl.com/api/Perfil/todos
        ///     
        ///     {
        ///         "nome": "string",
        ///         "dataNascimento": "1999-07-20T00:00:00",
        ///         "estado": "string",
        ///         "cidade": "string",
        ///         "bio": "string",
        ///         "avatarUrl": "string",
        ///         "login": {
        ///             "nomeUsuario": "string",
        ///             "email": "string",
        ///             "senha": "string",
        ///             "rotasPesquisadas": null,
        ///             "rotasRealizadas": null,
        ///             "avaliacoes": null,
        ///             "id": 0,
        ///             "atualizadoEm": "2018-03-26T17:29:48.5162586",
        ///             "atualizadoPor": "string",
        ///             "criadoEm": "0001-01-01T00:00:00",
        ///             "qtdAtualizacoes": 0
        ///         },
        ///         "idLogin": 0,
        ///         "id": 0,
        ///         "criadoEm": "2018-03-14T00:00:00",
        ///         "atualizadoEm": "2018-03-26T17:52:16.0957272",
        ///         "qtdAtualizacoes": 0,
        ///         "atualizadoPor": "string"
        ///     }
        /// 
        /// </remarks>
        /// <returns>lista de perfis cadastrados.</returns>
        /// <response code="200"> Retorna lista de perfis cadastrados.</response>
        /// <response code="400"> Ocorreu um erro.</response>
        [HttpGet]
        [Route("todos")]
        [EnableCors("AllowAnyOrigin")]
        //[Authorize("Bearer")]
        public IActionResult Buscar()
        {
            try {
                 return Ok(_perfilRepository.Listar(new string[]{"Login"}));
            } 
            catch(Exception ex)
            {
                return BadRequest("Erro ao buscar dados. " + ex.Message);
            }  
        }

        /// <summary>
        /// Retorna dados do Login juntamente com os dados básicos do perfil buscado pelo Id.
        /// </summary>
        /// <remarks>
        /// Exemplo de Retorno:
        /// 
        ///     GET http://brunohafonso-001-site1.ctempurl.com/api/Perfil/buscarid/{Id}
        ///     
        ///     {
        ///         "nome": "string",
        ///         "dataNascimento": "1999-07-20T00:00:00",
        ///         "estado": "string",
        ///         "cidade": "string",
        ///         "bio": "string",
        ///         "avatarUrl": "string",
        ///         "login": {
        ///             "nomeUsuario": "string",
        ///             "email": "string",
        ///             "senha": "string",
        ///             "rotasPesquisadas": null,
        ///             "rotasRealizadas": null,
        ///             "avaliacoes": null,
        ///             "id": 0,
        ///             "atualizadoEm": "2018-03-26T17:29:48.5162586",
        ///             "atualizadoPor": "string",
        ///             "criadoEm": "0001-01-01T00:00:00",
        ///             "qtdAtualizacoes": 0
        ///         },
        ///         "idLogin": 0,
        ///         "id": 0,
        ///         "criadoEm": "2018-03-14T00:00:00",
        ///         "atualizadoEm": "2018-03-26T17:52:16.0957272",
        ///         "qtdAtualizacoes": 0,
        ///         "atualizadoPor": "string"
        ///     }
        /// 
        /// </remarks>
        /// <param name="Id">Id do perfil a ser buscado na base de dados.</param>
        /// <returns>Objeto buscado caso exista algum registro com  Id persquisado.</returns>
        /// <response code="200"> Retorna dados do Login juntamente com os dados básicos do perfil buscado pelo Id.</response>
        /// <response code="400"> Ocorreu um erro.</response>
        /// <response code="404"> Nenhum perfil com esse Id cadastrado.</response>
        [HttpGet]
        [Route("buscarid/{Id}")]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult BuscarPorId(int Id)
        {
            try
            {
                var perfil = _perfilRepository.BuscarPorId(Id, new string[]{"Login"});
                if(perfil != null)
                    return Ok(perfil);
                else
                    return NotFound("não existe nenhum perfil com esse Id");
            }
            catch(Exception ex)
            {
                return BadRequest("Erro ao buscar dados. " + ex.Message);
            }
        }

        /// <summary>
        /// Efetua a atualização dos dados do Login juntamente com os dados básicos do perfil
        /// </summary>
        /// <remarks>
        /// Exemplo de Requisição:
        /// 
        ///     PUT http://brunohafonso-001-site1.ctempurl.com/api/Perfil/atualizar
        ///     
        ///     {
        ///         "nome": "string",
        ///         "dataNascimento": "1999-07-20T00:00:00",
        ///         "estado": "string",
        ///         "cidade": "string",
        ///         "bio": "string",
        ///         "avatarUrl": "string",
        ///         "idLogin": 0,
        ///         "id": 0,
        ///         "qtdAtualizacoes": 0
        ///     }
        /// 
        /// </remarks>
        /// <param name="perfil">Dados do login/perfil conforme criterios estabelecidos (precisa receber o objeto inteiro)</param>
        /// <returns>String informando qual objeto foi atualizado.</returns>
        /// <response code="200"> Retorna mensagem informand que o perfil foi atualizado.</response>
        /// <response code="400"> Ocorreu um erro.</response>
        [HttpPut]
        [Route("atualizar")]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult Atualizar([FromBody] Perfil perfil)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            try
            {
                perfil.AtualizadoEm = DateTime.Now;
                perfil.QtdAtualizacoes = perfil.QtdAtualizacoes + 1;
                perfil.AtualizadoPor = perfil.Nome;
                    
                _perfilRepository.Atualizar(perfil);
                return Ok($"Usuário {perfil.Nome} Atualizado Com Sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest("Erro ao atualizar os dados. " + ex.Message);
            }
        }
    }
}