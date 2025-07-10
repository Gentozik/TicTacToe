using TicTacToe.Datasource.Model;
using TicTacToe.Services;

namespace TicTacToe.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services)
        {
            services.AddSingleton<GameStorage>();
            services.AddScoped<IGameService, GameService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<AuthFilter>();

            return services;
        }
    }
}
