using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicTacToe.Datasource.Mapper;
using TicTacToe.Datasource.Model;
using TicTacToe.Domain.Model;

namespace TicTacToe.Services
{
    public class GameService(AppDbContext context) : IGameService
    {
        private readonly AppDbContext _context = context;
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
            if (winner == GameOutcome.FirstPlayerWon || winner == GameOutcome.SecondPlayerWon)
                return winner;

            for (int i = 0; i < GameBoard.Size; i++)
                for (int j = 0; j < GameBoard.Size; j++)
                    if (game.Board.BoardMatrix[i][j] == (int)PlayerEnum.None)
                        return game.GameOutcome;

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

            return game.GameOutcome;
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

        public async Task<GameDTO> CreateGameSolo([FromBody] Guid playerId)
        {
            var newGame = new Game(playerId, Guid.Empty);
            var gameDTO = DomainToDtoMapper.ToDTO(newGame);

            _context.Games.Add(gameDTO);
            await _context.SaveChangesAsync();

            return gameDTO;
        }
        public async Task<GameDTO> CreateGameMulti([FromBody] Guid playerId)
        {
            var newGame = new Game(playerId, null);
            var gameDTO = DomainToDtoMapper.ToDTO(newGame);

            _context.Games.Add(gameDTO);
            await _context.SaveChangesAsync();

            return gameDTO;
        }

        public async Task<IEnumerable<GameDTO>> GetAllGames()
        {
            IQueryable<GameDTO> query = _context.Games;

            query = query.Where(g => g.GameOutcome == GameOutcome.WaitingForPlayers);

            var games = await query.ToListAsync();

            return games;
        }

        public async Task<GameDTO?> GetGame(Guid id)
        {
            var game = await _context.Games.FindAsync(id);

            return game;
        }

        public async Task UpdateGame(Guid id, GameDTO updatedGame)
        {
            var existingGame = await _context.Games.FindAsync(id);

            _context.Games.Remove(existingGame);
            _context.Games.Add(updatedGame);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteGame(Guid id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
                return false;

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> JoinGame(Guid id, [FromBody] Guid playerId)
        {
            var game = await _context.Games.FindAsync(id);

            if (game == null)
                return false;

            if (game.PlayerOId != null && game.PlayerOId != Guid.Empty)
                return false;

            if (game.PlayerXId == playerId)
                return false;

            game.PlayerOId = playerId;
            game.GameOutcome = GameOutcome.FirstPlayerTurn;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task ClearAll()
        {
            _context.Games.RemoveRange(_context.Games);
            await _context.SaveChangesAsync();
        }
    }
}
