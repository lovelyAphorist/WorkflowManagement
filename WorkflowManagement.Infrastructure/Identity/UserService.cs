using Microsoft.AspNetCore.Identity;
using WorkflowManagement.Application.Users.Dtos;
using WorkflowManagement.Application.Users.Services;

namespace WorkflowManagement.Infrastructure.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<RegisterUserResult> RegisterAsync(
            RegisterUserRequest request)
        {
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                DisplayName = request.DisplayName.Trim(),
                UserName = request.Email.Trim(),
                Email = request.Email.Trim()
            };

            var result = await _userManager.CreateAsync(
                user,
                request.Password);

            if (!result.Succeeded)
            {
                return new RegisterUserResult
                {
                    Succeeded = false,
                    Errors = result.Errors
                        .Select(e => e.Description)
                        .ToList()
                };
            }

            return new RegisterUserResult
            {
                Succeeded = true,
                User = new UserResponse
                {
                    Id = user.Id,
                    DisplayName = user.DisplayName,
                    Email = user.Email!
                }
            };
        }
    }
}