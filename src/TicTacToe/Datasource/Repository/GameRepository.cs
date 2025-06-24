using TicTacToe.Datasource.Mapper;
using TicTacToe.Datasource.Model;
using TicTacToe.Domain.Model;

namespace TicTacToe.Datasource.Repository
{
    public class GameRepository(GameStorage storage) : IGameRepository
    {
        public async Task<Game?> Get(Guid id)
        {
            Game? val = null;
            GameDTO? gameDTO = await storage.GetGame(id);

            if (gameDTO != null)
                val = DomainToDtoMapper.ToDomain(gameDTO);

            return val;
        }

        public async Task Save(Game game)
        {
            ArgumentNullException.ThrowIfNull(game);

            var dto = DomainToDtoMapper.ToDTO(game);
            await storage.SaveGame(dto);
        }
    }
}