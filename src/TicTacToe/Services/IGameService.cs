using Microsoft.AspNetCore.Mvc;
using TicTacToe.Datasource.Model;
using TicTacToe.Domain.Model;

namespace TicTacToe.Services
{
    public interface IGameService
    {
        public Task<Game?> GetNextMove(Game game);

        public Task<bool> IsBoardValid(Game game, int row, int col);

        public GameOutcome HasGameEnded(Game game);
        public Task<GameDTO> CreateGameSolo([FromBody] Guid playerId);
        public Task<GameDTO> CreateGameMulti([FromBody] Guid playerId);
        public Task<IEnumerable<GameDTO>> GetAllGames();
        public Task<GameDTO?> GetGame(Guid id);
        public Task UpdateGame(Guid id, GameDTO updatedGame);
        public Task<bool> DeleteGame(Guid id);
        public Task<bool> JoinGame(Guid id, [FromBody] Guid playerId);
        public Task ClearAll();
    }
}
