using Auth_Services;
using Microsoft.AspNetCore.Mvc;
using UserEntity;

namespace Auth_Controller
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController
    {
        private readonly AuthServices _authService;

        public AuthController(AuthServices authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            var response = await _authService.CreateUser<Object>(user);
            return new JsonResult(response);
        }
    }
}
