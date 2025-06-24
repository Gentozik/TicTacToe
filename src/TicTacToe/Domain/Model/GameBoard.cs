namespace TicTacToe.Domain.Model
{
    public class GameBoard
    {
        public int[][] BoardMatrix { get; private set; }
        public const int Size = 3;
        public GameBoard()
        {
            BoardMatrix = new int[Size][];
            for (int i = 0; i < BoardMatrix.Length; i++)
            {
                BoardMatrix[i] = new int[Size];
            }
        }

        public GameBoard(int[][] board)
        {
            BoardMatrix = board;
        }
    }
}