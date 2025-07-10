using Microsoft.EntityFrameworkCore;
using System.Text;
using TicTacToe.Datasource.Model;
using TicTacToe.Domain.Model;

namespace TicTacToe.Services
{
    public class UserService(AppDbContext context) : IUserService
    {
        private readonly AppDbContext _context = context;
        public async Task<bool> RegisterUserAsync(SignUpRequest request)
        {
            var user = new UserDTO { Login = request.Login, Password = request.Password, Uuid = Guid.NewGuid().ToString() };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string?> AuthorizeUserAsync(string? authHeader)
        {
            if (authHeader == null)
            {
                return null;
            }

            string encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
            byte[] credentialBytes = Convert.FromBase64String(encodedCredentials);
            string decodedCredentials = Encoding.UTF8.GetString(credentialBytes);

            var parts = decodedCredentials.Split(':');

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == parts[0] && u.Password == parts[1]);

            if (user == null) {
                return null;
            }
            return user.Uuid;
        }

        public async Task<UserDTO?> GetUserAsync(string uuid)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Uuid == uuid);
        }

        public async Task ClearAll()
        {
            _context.Users.RemoveRange(_context.Users);
            await _context.SaveChangesAsync();
        }
    }
}
