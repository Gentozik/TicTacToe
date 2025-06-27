using TicTacToe.Datasource.Model;
using TicTacToe.Domain.Service;

namespace TicTacToe.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services)
        {
            services.AddSingleton<GameStorage>();
            services.AddScoped<IGameService, GameService>();

            return services;
        }
    }
}
