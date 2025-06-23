using TicTacToe.Datasource.Model;
using TicTacToe.Datasource.Repository;
using TicTacToe.Domain.Model;
using TicTacToe.Domain.Service;

namespace TicTacToe.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services)
        {
            services.AddSingleton<GameStorage>();
            services.AddScoped<IGameRepository, GameRepository>();
            services.AddScoped<IGameService, GameService>();

            return services;
        }
    }
}
