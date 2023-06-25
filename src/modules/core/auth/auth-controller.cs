using Auth_Services;
using Microsoft.AspNetCore.Mvc;
using AuthDTO;

namespace Auth_Controller
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthServices _authService;

        public AuthController(AuthServices authService)
        {
            _authService = authService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(AuthUserDTO authuser)
        {
            var response = await _authService.CreateUser<Object>(authuser);
            return new JsonResult(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(AuthUserDTO authuser)
        {
            var response = await _authService.LoginUser<Object>(authuser);
            return new JsonResult(response);
        }

    }
}
