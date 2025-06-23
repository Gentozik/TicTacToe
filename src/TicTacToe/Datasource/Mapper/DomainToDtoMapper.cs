using TicTacToe.Datasource.Model;
using TicTacToe.Domain.Model;

namespace TicTacToe.Datasource.Mapper
{
    public static class DomainToDtoMapper
    {
        public static Game ToDomain(GameDTO dto)
        {
            return new Game(dto.Id, new GameBoard(dto.GameBoard.BoardMatrix), dto.GameOutcome);
        }

        public static GameDTO ToDTO(Game game)
        {
            return new GameDTO
            {
                Id = game.Id,
                GameBoard = new GameBoardDTO(game.Board.BoardMatrix),
                GameOutcome = game.GameOutcome
            };
        }
    }
}
