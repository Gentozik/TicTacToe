using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TicTacToe.Domain.Model;
using TicTacToe.Domain.Service;
using TicTacToe.Web.Mapper;
using TicTacToe.Web.Model;

namespace TicTacToe.Web.Controllers;

[Route("game")]
public class WebController(IGameService gameService) : Controller
{
    private readonly IGameService _gameService = gameService;

    [HttpPost]
    [Route("new")]
    public async Task<IActionResult> NewGame()
    {
        Game game = new();
        await _gameService.SaveGame(game);
        return RedirectToAction("GetGame", new { guid = game.Id });
    }

    [HttpGet("{guid}")]
    public async Task<ActionResult<GameWebDTO>> GetGame(Guid guid)
    {
        Game? game = await _gameService.GetGame(guid);

        if (game == null)
        {
            return RedirectToPage("/Error", new { Code = 404 });
        }
        else
        {
            GameWebDTO gameWebDTO = DomainToWebMapper.ToWeb(game);
            return View(gameWebDTO);
        }
    }

    [HttpPost("/game/{id}")]
    public async Task<IActionResult> UpdateGame(Guid id, string action, string selectedCell, string boardMatrixJson)
    {
        if (string.IsNullOrEmpty(action) || string.IsNullOrEmpty(selectedCell) || string.IsNullOrEmpty(boardMatrixJson))
        {
            return RedirectToPage("/Error", new { Code = 400 });
        }

        var board = JsonSerializer.Deserialize<List<List<int>>>(boardMatrixJson);

        var dto = new GameWebDTO
        {
            Id = id,
            GameBoard = new GameBoardWebDTO(board!),
            GameOutcome = GameOutcome.None
        };

        var game = DomainToWebMapper.ToDomain(dto);

        int selectedCellInt = int.Parse(selectedCell);

        int row = selectedCellInt / 100;
        int col = selectedCellInt % 100;

        if (row < 0 || row >= GameBoard.Size || col < 0 || col >= GameBoard.Size)
        {
            return RedirectToPage("/Error", new { Code = 400 });
        }

        game.Board.BoardMatrix[row, col] = (int)PlayerEnum.FirstPlayer;

        if (_gameService.IsBoardValid(id, game).Result == false)
        {
            return RedirectToPage("/Error", new { Code = 400 });
        }

        await _gameService.SaveGame(game);

        game.GameOutcome = _gameService.HasGameEnded(game);
        if (game.GameOutcome == GameOutcome.None)
        {
            game = await _gameService.GetNextMove(game.Id);
            game.GameOutcome = _gameService.HasGameEnded(game);
        }

        await _gameService.SaveGame(game);

        return RedirectToAction("GetGame", new { guid = game.Id });
    }
}
