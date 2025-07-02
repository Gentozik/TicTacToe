using System.Collections.Concurrent;

namespace TicTacToe.Datasource.Model
{
    public class GameStorage
    {
        private readonly ConcurrentDictionary<Guid, GameDTO> _games = new();

        public async Task SaveGame(GameDTO game)
        {
            await Task.Run(() => _games[game.Id] = game);
        }

        public async Task<GameDTO?> GetGame(Guid id)
        {
            return await Task.Run(() =>
            {
                _games.TryGetValue(id, out var dto);
                return dto;
            });
        }
    }
}
