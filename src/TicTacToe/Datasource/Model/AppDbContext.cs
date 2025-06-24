using Microsoft.EntityFrameworkCore;

namespace TicTacToe.Datasource.Model
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<GameDTO> Games { get; set; }
    }
}