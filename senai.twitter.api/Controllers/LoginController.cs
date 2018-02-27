using Microsoft.AspNetCore.Mvc;
using senai.twitter.domain.Contracts;
using senai.twitter.domain.Entities;

namespace senai.twitter.api.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private IBaseRepository<Login> _loginRepository;

        public LoginController(IBaseRepository<Login> loginRepository)
        {

            _loginRepository = loginRepository;
        }

        [HttpGet]
        public IActionResult GetAction()
        {
            return Ok(_loginRepository.Listar(new string[]{"Perfil"}));
        }

        [HttpGet("{id}")]
        public IActionResult GetAction(int id)
        {
            var login = _loginRepository.BuscarPorId(id);
            if (login != null)
                return Ok(login);
            else
                return NotFound();
        }
    }
}