using Microsoft.AspNetCore.Mvc;
using Services;
using UserEntity;

namespace Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            var response = await _userService.CreateUser<Object>(user);
            return new JsonResult(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var response = await _userService.GetUser<Object>(id);
            return new JsonResult(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, User user)
        {
            var response = await _userService.UpdateUser<Object>(id, user);
            return new JsonResult(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var response = await _userService.DeleteUser<Object>(id);
            return new JsonResult(response);
        }
    }
}
