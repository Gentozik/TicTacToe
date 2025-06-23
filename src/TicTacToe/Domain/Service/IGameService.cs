using TicTacToe.Domain.Model;

namespace TicTacToe.Domain.Service
{
    public interface IGameService
    {
        public Task<Game?> GetGame(Guid id);
        public Task SaveGame(Game game);
        public Task<Game?> GetNextMove(Guid id);

        public Task<bool> IsBoardValid(Guid guid, Game gameWithNextMove);

        public GameOutcome HasGameEnded(Game game);
    }
}
