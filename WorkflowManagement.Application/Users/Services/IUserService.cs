using WorkflowManagement.Application.Users.Dtos;

namespace WorkflowManagement.Application.Users.Services
{
    public interface IUserService
    {
        Task<RegisterUserResult> RegisterAsync(RegisterUserRequest request);
        Task<LoginResult> LoginAsync(LoginRequest request);
        Task<IReadOnlyList<UserResponse>> GetAllAsync();
        Task<UserResponse?> GetByIdAsync(Guid id);
    }
}