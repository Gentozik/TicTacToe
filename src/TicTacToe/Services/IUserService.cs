using TicTacToe.Datasource.Model;
using TicTacToe.Domain.Model;

namespace TicTacToe.Services
{
    public interface IUserService
    {
        public Task<bool> RegisterUserAsync(SignUpRequest request);
        public Task<string?> AuthorizeUserAsync(string? authHeader);
        public Task<UserDTO?> GetUserAsync(string uuid);
        public Task ClearAll();
    }
}
