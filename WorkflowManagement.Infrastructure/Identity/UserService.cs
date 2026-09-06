using Microsoft.AspNetCore.Identity;
using WorkflowManagement.Application.Users.Dtos;
using WorkflowManagement.Application.Users.Services;

namespace WorkflowManagement.Infrastructure.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;

        public UserService(
            UserManager<ApplicationUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
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
        public async Task<LoginResult> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(
                request.Email.Trim());

            if (user is null)
            {
                return InvalidLogin();
            }

            var passwordValid =
                await _userManager.CheckPasswordAsync(
                    user,
                    request.Password);

            if (!passwordValid)
            {
                return InvalidLogin();
            }

            var token = _tokenService.GenerateToken(
                user.Id,
                user.Email!,
                user.DisplayName);

            return new LoginResult
            {
                Succeeded = true,
                Token = token.Token,
                ExpiresAtUtc = token.ExpiresAtUtc,
                User = new UserResponse
                {
                    Id = user.Id,
                    DisplayName = user.DisplayName,
                    Email = user.Email!
                }
            };
        }
        private static LoginResult InvalidLogin()
        {
            return new LoginResult
            {
                Succeeded = false,
                Errors = new List<string>
        {
            "Invalid email or password."
        }
            };
        }
    }
}