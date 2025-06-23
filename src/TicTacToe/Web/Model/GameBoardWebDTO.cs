namespace TicTacToe.Web.Model
{
    public class GameBoardWebDTO
    {
        public List<List<int>> BoardMatrix { get; init; } = [];

        public GameBoardWebDTO()
        {
            BoardMatrix = [];
        }

        public GameBoardWebDTO(List<List<int>> board)
        {
            BoardMatrix = board;
        }
    }
}
