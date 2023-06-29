using Microsoft.AspNetCore.Mvc;
using Services;
using UserEntity;
using Microsoft.AspNetCore.Authorization;
using UserDTO;

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

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser(UserUpdateDTO updateUser)
        {
            var response = await _userService.UpdateUser<Object>(updateUser);
            return new JsonResult(response);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUser(Guid? optionalId = null)
        {
            var response = await _userService.DeleteUser<Object>(optionalId);
            return new JsonResult(response);
        }
    }
}
