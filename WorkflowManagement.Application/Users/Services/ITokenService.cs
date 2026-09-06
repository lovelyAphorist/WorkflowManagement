
using WorkflowManagement.Application.Users.Dtos;

namespace WorkflowManagement.Application.Users.Services
{
    public interface ITokenService
    {
        TokenResult GenerateToken(Guid userId, string email, string displayName);
    }
}