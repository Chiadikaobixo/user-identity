using Microsoft.AspNetCore.Mvc;
using Services;
using UserEntity;
using Microsoft.AspNetCore.Authorization;

namespace Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
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
