using TicTacToe.Domain.Model;

public class Game
{
    public Guid Id { get; private set; }
    public GameBoard Board { get; private set; }
    public GameOutcome GameOutcome { get; set; } = GameOutcome.None;
    public Guid PlayerXId { get; set; }
    public Guid? PlayerOId { get; set; }

    public Game(Guid id, GameBoard board, GameOutcome outcome, Guid playerXId, Guid? playerOId = null)
    {
        Id = id;
        Board = board;
        GameOutcome = outcome;
        PlayerXId = playerXId;
        PlayerOId = playerOId;
    }

    public Game(Guid playerXId, Guid? playerOId)
    {
        Id = Guid.NewGuid();
        Board = new GameBoard();
        PlayerXId = playerXId;
        if (playerOId == null) {
            GameOutcome = GameOutcome.WaitingForPlayers;
        } else {
            GameOutcome = GameOutcome.None;
        }
        PlayerOId = playerOId;
    }

    public Game()
    {
        Id = Guid.NewGuid();
        Board = new GameBoard();
    }
}