using TicTacToe.Domain.Model;

namespace TicTacToe.Domain.Service
{
    public interface IGameService
    {
        public Task<Game?> GetGame(Guid id);
        public Task SaveGame(Game game);
        public Task<Game?> GetNextMove(Game game);

        public Task<bool> IsBoardValid(Game game, int row, int col);

        public GameOutcome HasGameEnded(Game game);
    }
}
