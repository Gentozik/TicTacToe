using TicTacToe.Domain.Model;

namespace TicTacToe.Datasource.Model
{
    public class GameBoardDTO
    {
        public int[,] BoardMatrix { get; private set; }

        public GameBoardDTO()
        {
            BoardMatrix = new int[GameBoard.Size, GameBoard.Size];
        }

        public GameBoardDTO(int[,] board)
        {
            BoardMatrix = board;
        }
    }
}
