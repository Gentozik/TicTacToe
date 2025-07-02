using TicTacToe.Datasource.Model;

namespace TicTacToe.Services
{
    public interface IUserService
    {
        public Task<UserDTO> RegisterUserAsync(string login, string password);
        public Task<UserDTO?> AuthorizeUserAsync(string login, string password);
    }
}
