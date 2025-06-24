using Microsoft.EntityFrameworkCore;

namespace TicTacToe.Datasource.Model
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<GameDTO> Games { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameDTO>(entity =>
            {
                entity.OwnsOne(g => g.GameBoard);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}