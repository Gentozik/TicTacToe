namespace TicTacToe.Domain.Model
{
    public interface IGameRepository
    {
        public Task Save(Game game);

        public Task<Game?> Get(Guid id);
    }
}
