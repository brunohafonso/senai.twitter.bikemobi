using Microsoft.AspNetCore.Mvc;
using senai.twitter.domain.Contracts;
using senai.twitter.domain.Entities;

namespace senai.twitter.api.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private IBaseRepository<Login> _loginRepository;
        private IBaseRepository<Perfil> _perfilRepository;

        public LoginController(IBaseRepository<Login> loginRepository,IBaseRepository<Perfil> perfilRepository)
        {

            _loginRepository = loginRepository;
            _perfilRepository = perfilRepository;
        }

        [HttpGet]
        public IActionResult Buscar()
        {
            return Ok(_loginRepository.Listar(new string[]{"Perfil"}));
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

        [HttpPost]
        public IActionResult Cadastrar([FromBody] Login login)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(_loginRepository.Inserir(login));
        }

        [HttpPut]
        public IActionResult Atualizar([FromBody] Login login)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(_loginRepository.Atualizar(login));
        }

        [HttpDelete]
        public IActionResult Deletar([FromBody] Login login)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(_loginRepository.Deletar(login));
        }
    }
}