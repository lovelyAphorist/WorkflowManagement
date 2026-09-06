using Microsoft.AspNetCore.Mvc;
using WorkflowManagement.Application.Users.Dtos;
using WorkflowManagement.Application.Users.Services;

namespace WorkflowManagement.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponse>> Register(RegisterUserRequest request)
        {
            var result = await _userService.RegisterAsync(request);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    errors = result.Errors
                });
            }

            return StatusCode(StatusCodes.Status201Created, result.User);
        }
    }
}