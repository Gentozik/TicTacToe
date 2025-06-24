using System.ComponentModel.DataAnnotations.Schema;

[ComplexType]
public class GameBoardDTO
{
    public int[,] BoardMatrix { get; private set; }

    public GameBoardDTO()
    {
        BoardMatrix = new int[3, 3];
    }

    public GameBoardDTO(int[,] board)
    {
        BoardMatrix = board;
    }
}
