using TicTacToe.Domain.Model;

namespace TicTacToe.Services
{
    public class GameService() : IGameService
    {
        public async Task<Game?> GetNextMove(Game game)
        {

            if (game != null)
            {
                int bestScore = int.MinValue;
                int bestRow = 0;
                int bestCol = 0;

                for (int i = 0; i < GameBoard.Size; i++)
                {
                    for (int j = 0; j < GameBoard.Size; j++)
                    {
                        if (game.Board.BoardMatrix[i][j] == 0)
                        {
                            game.Board.BoardMatrix[i][j] = (int)PlayerEnum.SecondPlayer;
                            int result = MinMaxCalculate(game, (int)PlayerEnum.FirstPlayer);
                            game.Board.BoardMatrix[i][j] = (int)PlayerEnum.None;

                            if (result > bestScore)
                            {
                                bestScore = result;
                                bestRow = i;
                                bestCol = j;
                            }
                        }
                    }
                }
                game.Board.BoardMatrix[bestRow][bestCol] = (int)PlayerEnum.SecondPlayer;
            }

            return game;
        }

        public int MinMaxCalculate(Game game, int currentPlayer)
        {
            var outcome = HasGameEnded(game);

            if (outcome == GameOutcome.Draw) return 0;
            if (outcome == GameOutcome.FirstPlayerWon) return -1;
            if (outcome == GameOutcome.SecondPlayerWon) return 1;

            int bestScore = currentPlayer == (int)PlayerEnum.SecondPlayer ? int.MinValue : int.MaxValue;

            for (int i = 0; i < GameBoard.Size; i++)
            {
                for (int j = 0; j < GameBoard.Size; j++)
                {
                    if (game.Board.BoardMatrix[i][j] == (int)PlayerEnum.None)
                    {
                        game.Board.BoardMatrix[i][j] = currentPlayer;
                        int score = MinMaxCalculate(game, currentPlayer == (int)PlayerEnum.FirstPlayer ? (int)PlayerEnum.SecondPlayer : (int)PlayerEnum.FirstPlayer);
                        game.Board.BoardMatrix[i][j] = (int)PlayerEnum.None;

                        if (currentPlayer == (int)PlayerEnum.SecondPlayer)
                            bestScore = Math.Max(score, bestScore);
                        else
                            bestScore = Math.Min(score, bestScore);
                    }
                }
            }

            return bestScore;
        }


        public GameOutcome HasGameEnded(Game game)
        {
            var winner = FindGameWinner(game);
            if (winner != GameOutcome.None)
                return winner;

            for (int i = 0; i < GameBoard.Size; i++)
                for (int j = 0; j < GameBoard.Size; j++)
                    if (game.Board.BoardMatrix[i][j] == (int)PlayerEnum.None)
                        return GameOutcome.None;

            return GameOutcome.Draw;
        }

        private GameOutcome FindGameWinner(Game game)
        {
            for (int i = 0; i < GameBoard.Size; i++)
            {
                if (game.Board.BoardMatrix[i][0] != (int)PlayerEnum.None &&
                    game.Board.BoardMatrix[i][0] == game.Board.BoardMatrix[i][1] &&
                    game.Board.BoardMatrix[i][1] == game.Board.BoardMatrix[i][2])
                {
                    return game.Board.BoardMatrix[i][0] == (int)PlayerEnum.FirstPlayer ? GameOutcome.FirstPlayerWon : GameOutcome.SecondPlayerWon;
                }
            }

            for (int j = 0; j < GameBoard.Size; j++)
            {
                if (game.Board.BoardMatrix[0][j] != (int)PlayerEnum.None &&
                    game.Board.BoardMatrix[0][j] == game.Board.BoardMatrix[1][j] &&
                    game.Board.BoardMatrix[1][j] == game.Board.BoardMatrix[2][j])
                {
                    return game.Board.BoardMatrix[0][j] == (int)PlayerEnum.FirstPlayer ? GameOutcome.FirstPlayerWon : GameOutcome.SecondPlayerWon;
                }
            }

            if (game.Board.BoardMatrix[0][0] != (int)PlayerEnum.None &&
                game.Board.BoardMatrix[0][0] == game.Board.BoardMatrix[1][1] &&
                game.Board.BoardMatrix[1][1] == game.Board.BoardMatrix[2][2])
            {
                return game.Board.BoardMatrix[0][0] == (int)PlayerEnum.FirstPlayer ? GameOutcome.FirstPlayerWon : GameOutcome.SecondPlayerWon;
            }

            if (game.Board.BoardMatrix[0][0] != (int)PlayerEnum.None &&
                game.Board.BoardMatrix[0][2] == game.Board.BoardMatrix[1][1] &&
                game.Board.BoardMatrix[1][1] == game.Board.BoardMatrix[2][0])
            {
                return game.Board.BoardMatrix[0][2] == (int)PlayerEnum.FirstPlayer ? GameOutcome.FirstPlayerWon : GameOutcome.SecondPlayerWon;
            }

            return GameOutcome.None;
        }

        public async Task<bool> IsBoardValid(Game game, int row, int col)
        {
            if (game == null)
            {
                return false;
            }
            else if (row < 0 || row >= GameBoard.Size || col < 0 || col >= GameBoard.Size)
            {
                return false;
            }

            return true;
        }
    }
}
