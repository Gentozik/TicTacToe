namespace TicTacToe.Domain.Model
{
    public enum GameOutcome
    {
        None,
        Draw,
        FirstPlayerWon,
        SecondPlayerWon,
        WaitingForPlayers,
        FirstPlayerTurn,
        SecondPlayerTurn
    }
}
