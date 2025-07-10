using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using TicTacToe.Datasource.Mapper;
using TicTacToe.Domain.Model;
using TicTacToe.Services;
using TicTacToe.Web.Mapper;
using TicTacToe.Web.Model;

namespace TicTacToe.Web.Controllers;

[Route("game")]
public class WebController(IGameService gameService) : Controller
{
    private readonly IGameService _gameService = gameService;

    [HttpGet("/")]
    public IActionResult Index()
    {
        return View();
    }

    [ServiceFilter(typeof(AuthFilter))]
    [HttpPost]
    [Route("new-solo")]
    public async Task<IActionResult> NewGame()
    {
        HttpContext.Items.TryGetValue("UserId", out var userIdString);

        var game = await _gameService.CreateGameSolo(new Guid(userIdString.ToString()));

        return RedirectToAction("GetGame", new { guid = game.Id });
    }

    [ServiceFilter(typeof(AuthFilter))]
    [HttpPost]
    [Route("new-multi")]
    public async Task<IActionResult> NewGameMulti()
    {
        HttpContext.Items.TryGetValue("UserId", out var userIdString);

        var game = await _gameService.CreateGameMulti(new Guid(userIdString.ToString()));

        return RedirectToAction("GetGame", new { guid = game.Id });
    }

    [ServiceFilter(typeof(AuthFilter))]
    [HttpGet("{guid}")]
    public async Task<IActionResult> GetGame(Guid guid)
    {
        var game = await _gameService.GetGame(guid);

        if (game == null)
            return RedirectToPage("/Error", new { Code = 404 });

        var webGame = DomainToWebMapper.ToWeb(DomainToDtoMapper.ToDomain(game));

        return View(webGame);
    }

    [ServiceFilter(typeof(AuthFilter))]
    [HttpPost("/game/{id}")]
    public async Task<IActionResult> UpdateGame(Guid id, string action, string selectedCell, string boardMatrixJson)
    {
        if (string.IsNullOrEmpty(action) || string.IsNullOrEmpty(selectedCell) || string.IsNullOrEmpty(boardMatrixJson))
        {
            return RedirectToPage("/Error", new { Code = 400 });
        }

        string? userId = HttpContext.Session.GetString("UserId");
        if (userId == null || !Guid.TryParse(userId, out var currentUserId))
        {
            return RedirectToPage("/Error", new { Code = 401 });
        }

        var board = JsonSerializer.Deserialize<List<List<int>>>(boardMatrixJson);
        if (board == null)
        {
            return RedirectToPage("/Error", new { Code = 400 });
        }

        var gameResponse = await _gameService.GetGame(id);
        if (gameResponse == null)
        {
            return RedirectToPage("/Error", new { Code = 404 });
        }

        var game = DomainToDtoMapper.ToDomain(gameResponse);
        int row = int.Parse(selectedCell) / 100;
        int col = int.Parse(selectedCell) % 100;

        bool isBoardValid = await _gameService.IsBoardValid(game, row, col);
        if (!isBoardValid)
        {
            return RedirectToPage("/Error", new { Code = 400 });
        }

        var isMultiplayer = game.PlayerOId.HasValue && game.PlayerOId.Value != Guid.Empty;
        var currentOutcome = game.GameOutcome;

        if ((currentOutcome == GameOutcome.FirstPlayerTurn && currentUserId == game.PlayerXId) ||
            (currentOutcome == GameOutcome.SecondPlayerTurn && currentUserId == game.PlayerOId))
        {
            game.Board.BoardMatrix[row][col] = currentOutcome == GameOutcome.FirstPlayerTurn ? 1 : 2;

            game.GameOutcome = _gameService.HasGameEnded(game);

            if (game.GameOutcome == GameOutcome.FirstPlayerTurn || game.GameOutcome == GameOutcome.SecondPlayerTurn)
            {
                if (isMultiplayer)
                {
                    game.GameOutcome = currentOutcome == GameOutcome.FirstPlayerTurn
                        ? GameOutcome.SecondPlayerTurn
                        : GameOutcome.FirstPlayerTurn;
                }
                else
                {
                    game.GameOutcome = GameOutcome.FirstPlayerTurn;
                    game = await _gameService.GetNextMove(game);
                    game.GameOutcome = _gameService.HasGameEnded(game);
                }
            }

            var updatedDTO = DomainToDtoMapper.ToDTO(game);
            await _gameService.UpdateGame(game.Id, updatedDTO);
        }
        else
        {
            return RedirectToPage("/Error", new { Code = 403 });
        }

        return RedirectToAction("GetGame", new { guid = game.Id });
    }

    [ServiceFilter(typeof(AuthFilter))]
    [HttpGet("game/all")]
    public async Task<IActionResult> AllGames()
    {
        var gameList = await gameService.GetAllGames();

        var dtoList = gameList.ToList();

        if (dtoList != null)
        {
            var webList = new List<GameWebDTO>();
            var domainList = new List<Game>();
            foreach (var dto in dtoList)
            {
                webList.Add(DomainToWebMapper.ToWeb(DomainToDtoMapper.ToDomain(dto)));
                domainList.Add(DomainToDtoMapper.ToDomain(dto));
            }

            return View(webList);
        }

        return RedirectToPage("/Error", new { Code = 404 });
    }

    [ServiceFilter(typeof(AuthFilter))]
    [HttpPost]
    [Route("delete/{id}")]
    public async Task<IActionResult> DeleteGame(Guid id)
    {
        var result = await _gameService.DeleteGame(id);

        if (result)
        {
            return RedirectToAction("AllGames");
        }
        else
        {
            return RedirectToPage("/Error", new { Code = 404 });
        }
    }

    [ServiceFilter(typeof(AuthFilter))]
    [HttpPost("JoinGame")]
    public async Task<IActionResult> JoinGame(Guid id)
    {
        string? userIdString = HttpContext.Session.GetString("UserId");

        if (!Guid.TryParse(userIdString, out var userId))
            return RedirectToPage("/Error", new { Code = 403 });

        var result = await _gameService.JoinGame(id, userId);

        if (result)
        {
            return RedirectToAction("GetGame", new { guid = id });
        }

        return RedirectToPage("/Error", new { Code = 400 });
    }
}
