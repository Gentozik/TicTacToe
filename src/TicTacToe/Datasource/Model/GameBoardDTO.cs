using System.Text.Json;
using System.ComponentModel.DataAnnotations.Schema;
using TicTacToe.Domain.Model;

public class GameBoardDTO
{
    [NotMapped]
    public int[][] BoardMatrix { get; set; }

    public string BoardMatrixJson
    {
        get => JsonSerializer.Serialize(BoardMatrix);
        set => BoardMatrix = string.IsNullOrEmpty(value)
            ? null
            : JsonSerializer.Deserialize<int[][]>(value);
    }

    public GameBoardDTO()
    {
        BoardMatrix = new int[GameBoard.Size][];
        for (int i = 0; i < GameBoard.Size; i++)
            BoardMatrix[i] = new int[GameBoard.Size];
    }

    public GameBoardDTO(int[][] board)
    {
        BoardMatrix = new int[GameBoard.Size][];
        for (int i = 0; i < GameBoard.Size; i++)
        {
            BoardMatrix[i] = new int[GameBoard.Size];
            for (int j = 0; j < GameBoard.Size; j++)
            {
                BoardMatrix[i][j] = board[i][j];
            }
        }
    }
}
