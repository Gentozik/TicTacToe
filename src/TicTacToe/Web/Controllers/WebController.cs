using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text;
using System.Text.Json;
using TicTacToe.Datasource.Mapper;
using TicTacToe.Datasource.Model;
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
        //Console.WriteLine("[WebController]: Попытка создания игры");
        using var client = new HttpClient();
        var response = await client.PostAsync("http://localhost:5194/api/GamesDB/create", null);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);
            var game = JsonSerializer.Deserialize<GameDTO>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            //Console.WriteLine($"[WebController]: Игра создана, Id: {game.Id}");
            return RedirectToAction("GetGame", new { guid = game.Id });
        }
        else
        {
            Console.WriteLine($"[WebController]: Ошибка создания игры");
            return RedirectToPage("/Error", new { Code = 404 });
        }
    }

    [HttpGet("{guid}")]
    public async Task<IActionResult> GetGame(Guid guid)
    {
        using var client = new HttpClient();
        var response = await client.GetAsync($"http://localhost:5194/api/GamesDB/get/{guid}");

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            var game = JsonSerializer.Deserialize<GameDTO>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(DomainToWebMapper.ToWeb(DomainToDtoMapper.ToDomain(game)));
        }
        else
        {
            Console.WriteLine($"[WebController]: Ошибка получения игры");
            return RedirectToPage("/Error", new { Code = 404 });
        }
    }

    [HttpPost("/game/{id}")]
    public async Task<IActionResult> UpdateGame(Guid id, string action, string selectedCell, string boardMatrixJson)
    {
        if (string.IsNullOrEmpty(action) || string.IsNullOrEmpty(selectedCell) || string.IsNullOrEmpty(boardMatrixJson))
        {
            Console.WriteLine($"[WebController]: Ошибка принятия запроса на ход");
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

        if (_gameService.IsBoardValid(game, row, col).Result == false)
        {
            Console.WriteLine($"[WebController]: Невалидная доска");
            return RedirectToPage("/Error", new { Code = 400 });
        }
        else
        {
            game.Board.BoardMatrix[row][col] = (int)PlayerEnum.FirstPlayer;
        }

        game.GameOutcome = _gameService.HasGameEnded(game);
        if (game.GameOutcome == GameOutcome.None)
        {
            game = await _gameService.GetNextMove(game);
            game.GameOutcome = _gameService.HasGameEnded(game);
        }

        using var client = new HttpClient();
        var json = JsonSerializer.Serialize(DomainToDtoMapper.ToDTO(game));
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        await client.PutAsync($"http://localhost:5194/api/GamesDB/update/{game.Id}", content);

        return RedirectToAction("GetGame", new { guid = game.Id });
    }

    [HttpGet]
    public async Task<IActionResult> AllGames()
    {
        using var client = new HttpClient();
        var response = await client.GetAsync("http://localhost:5194/api/GamesDB/getList");

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var dtoList = JsonSerializer.Deserialize<List<GameDTO>>(json);

            // Преобразование GameDTO → GameWebDTO
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

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<IActionResult> DeleteGame(Guid id)
    {
        //Console.WriteLine("[WebController]: Попытка создания игры");
        using var client = new HttpClient();
        var response = await client.DeleteAsync($"http://localhost:5194/api/GamesDB/delete/{id}");

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("AllGames");
        }
        else
        {
            Console.WriteLine($"[WebController]: Ошибка удаления игры");
            return RedirectToPage("/Error", new { Code = 404 });
        }
    }
}
