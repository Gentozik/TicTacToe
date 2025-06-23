using TicTacToe.Web.Model;
using TicTacToe.Domain.Model;

namespace TicTacToe.Web.Mapper
{
    public static class DomainToWebMapper
    {
        public static Game ToDomain(GameWebDTO dto)
        {
            var matrix = new int[GameBoard.Size, GameBoard.Size];
            for (int i = 0; i < GameBoard.Size; i++)
            {
                for (int j = 0; j < GameBoard.Size; j++)
                {
                    matrix[i, j] = dto.GameBoard.BoardMatrix[i][j];
                }
            }

            return new Game(dto.Id, new GameBoard(matrix), dto.GameOutcome);
        }

        public static GameWebDTO ToWeb(Game game)
        {
            var boardList = new List<List<int>>();
            for (int i = 0; i < GameBoard.Size; i++)
            {
                var row = new List<int>();
                for (int j = 0; j < GameBoard.Size; j++)
                {
                    row.Add(game.Board.BoardMatrix[i, j]);
                }
                boardList.Add(row);
            }

            return new GameWebDTO
            {
                Id = game.Id,
                GameBoard = new GameBoardWebDTO(boardList),
                GameOutcome = game.GameOutcome
            };
        }
    }
}
