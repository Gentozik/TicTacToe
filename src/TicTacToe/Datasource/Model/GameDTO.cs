using TicTacToe.Domain.Model;

namespace TicTacToe.Datasource.Model
{
    public class GameDTO
    {
        public Guid Id { get; init; } = new Guid();
        public GameBoardDTO GameBoard { get; init; } = new GameBoardDTO();
        public GameOutcome GameOutcome { get; init; } = GameOutcome.None;
    }
}
