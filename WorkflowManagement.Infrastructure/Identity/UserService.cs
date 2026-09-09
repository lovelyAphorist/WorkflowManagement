using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkflowManagement.Application.Users.Constants;
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

            var roleResult = await _userManager.AddToRoleAsync(
    user,
    AppRoles.Member);

            if (!roleResult.Succeeded)
            {
                return new RegisterUserResult
                {
                    Succeeded = false,
                    Errors = roleResult.Errors
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
            var roles = await _userManager.GetRolesAsync(user);

            var token = _tokenService.GenerateToken(
                user.Id,
                user.Email!,
                user.DisplayName,
                roles);

            return new LoginResult
            {
                Succeeded = true,
                Token = token.Token,
                ExpiresAtUtc = token.ExpiresAtUtc,
                Roles = roles.ToList(),
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

        public async Task<IReadOnlyList<UserResponse>> GetAllAsync()
        {
            return await _userManager.Users
                .AsNoTracking()
                .OrderBy(u => u.DisplayName)
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    DisplayName = u.DisplayName,
                    Email = u.Email!
                })
                .ToListAsync();
        }

        public async Task<UserResponse?> GetByIdAsync(Guid id)
        {
            return await _userManager.Users
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    DisplayName = u.DisplayName,
                    Email = u.Email!
                })
                .SingleOrDefaultAsync();
        }

        public async Task<IReadOnlyList<UserResponse>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            var userIds = ids
                .Distinct()
                .ToList();

            return await _userManager.Users
                .AsNoTracking()
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    DisplayName = u.DisplayName,
                    Email = u.Email!
                })
                .ToListAsync();
        }
    }
}