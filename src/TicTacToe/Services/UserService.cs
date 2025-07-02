using Microsoft.EntityFrameworkCore;
using TicTacToe.Datasource.Model;
using TicTacToe.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context) => _context = context;

    public async Task<UserDTO> RegisterUserAsync(string login, string password)
    {
        var user = new UserDTO { Login = login, Password = password };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<UserDTO?> AuthorizeUserAsync(string login, string password)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Login == login && u.Password == password);
    }
}