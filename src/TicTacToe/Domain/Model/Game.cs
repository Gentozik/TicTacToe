using TicTacToe.Domain.Model;

public class Game
{
    public Guid Id { get; private set; }
    public GameBoard Board { get; private set; }
    public GameOutcome GameOutcome { get; set; } = GameOutcome.None;

    public Game(Guid id, GameBoard board, GameOutcome outcome)
    {
        Id = id;
        Board = board;
        GameOutcome = outcome;
    }

    public Game()
    {
        Id = Guid.NewGuid();
        Board = new GameBoard();
    }
}
