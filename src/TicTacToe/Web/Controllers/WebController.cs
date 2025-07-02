using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using TicTacToe.Datasource.Mapper;
using TicTacToe.Datasource.Model;
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
    public IActionResult Index() => View();

    [HttpPost]
    [Route("new-solo")]
    public async Task<IActionResult> NewGame()
    {
        using var client = new HttpClient();
        var response = await client.PostAsync("http://localhost:5194/api/GamesDB/create-solo", null);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);
            var game = JsonSerializer.Deserialize<GameDTO>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return RedirectToAction("GetGame", new { guid = game.Id });
        }
        else
        {
            Console.WriteLine($"[WebController]: Ошибка создания игры");
            return RedirectToPage("/Error", new { Code = 404 });
        }
    }

    [HttpPost]
    [Route("new-multi")]
    public async Task<IActionResult> NewGameMulti()
    {
        using var client = new HttpClient();
        var response = await client.PostAsync("http://localhost:5194/api/GamesDB/create-multi", null);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);
            var game = JsonSerializer.Deserialize<GameDTO>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return RedirectToAction("GetGame", new { guid = game.Id });
        }
        else
        {
            Console.WriteLine($"[WebController]: Ошибка создания мультиплеерной игры");
            return RedirectToPage("/Error", new { Code = 404 });
        }
    }

    [HttpGet("{guid}")]
    public async Task<IActionResult> GetGame(Guid guid)
    {
        using var client = new HttpClient();
        var response = await client.GetAsync($"http://localhost:5194/api/GamesDB/get/{guid}");

        if (!response.IsSuccessStatusCode)
            return RedirectToPage("/Error", new { Code = 404 });

        var result = await response.Content.ReadAsStringAsync();
        var game = JsonSerializer.Deserialize<GameDTO>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var webGame = DomainToWebMapper.ToWeb(DomainToDtoMapper.ToDomain(game));
        ViewData["CurrentUserId"] = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return View(webGame);
    }

    [HttpPost("/game/{id}")]
    public async Task<IActionResult> UpdateGame(Guid id, string action, string selectedCell, string boardMatrixJson)
    {
        using var client = new HttpClient();

        if (string.IsNullOrEmpty(action) || string.IsNullOrEmpty(selectedCell) || string.IsNullOrEmpty(boardMatrixJson))
        {
            Console.WriteLine($"[WebController]: Ошибка принятия запроса на ход");
            return RedirectToPage("/Error", new { Code = 400 });
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var currentUserId))
        {
            Console.WriteLine("[WebController]: Пользователь не авторизован");
            return RedirectToPage("/Error", new { Code = 401 });
        }

        var board = JsonSerializer.Deserialize<List<List<int>>>(boardMatrixJson);
        if (board == null)
        {
            Console.WriteLine("[WebController]: Не удалось десериализовать доску");
            return RedirectToPage("/Error", new { Code = 400 });
        }

        var gameResponse = await client.GetAsync($"http://localhost:5194/api/GamesDB/get/{id}");
        if (!gameResponse.IsSuccessStatusCode)
        {
            Console.WriteLine($"[WebController]: Игра {id} не найдена");
            return RedirectToPage("/Error", new { Code = 404 });
        }

        var gameDtoJson = await gameResponse.Content.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize<GameDTO>(gameDtoJson);

        if (dto == null)
        {
            Console.WriteLine("[WebController]: Не удалось распарсить GameDTO");
            return RedirectToPage("/Error", new { Code = 500 });
        }

        var game = DomainToDtoMapper.ToDomain(dto);
        int row = int.Parse(selectedCell) / 100;
        int col = int.Parse(selectedCell) % 100;

        if (!_gameService.IsBoardValid(game, row, col).Result)
        {
            Console.WriteLine("[WebController]: Ход в занятое поле");
            return RedirectToPage("/Error", new { Code = 400 });
        }

        var isMultiplayer = game.PlayerOId.HasValue && game.PlayerOId.Value != Guid.Empty;
        var currentOutcome = game.GameOutcome;

        if ((currentOutcome == GameOutcome.FirstPlayerTurn && currentUserId == game.PlayerOId) ||
            (currentOutcome == GameOutcome.SecondPlayerTurn && currentUserId == game.PlayerXId))
        {
            game.Board.BoardMatrix[row][col] = currentOutcome == GameOutcome.FirstPlayerTurn ? 1 : 2;

            game.GameOutcome = _gameService.HasGameEnded(game);

            if (game.GameOutcome == GameOutcome.None)
            {
                if (isMultiplayer)
                {
                    game.GameOutcome = currentOutcome == GameOutcome.FirstPlayerTurn
                        ? GameOutcome.SecondPlayerTurn
                        : GameOutcome.FirstPlayerTurn;
                }
                else
                {
                    game = await _gameService.GetNextMove(game);
                    game.GameOutcome = _gameService.HasGameEnded(game);
                }
            }

            var updatedJson = JsonSerializer.Serialize(DomainToDtoMapper.ToDTO(game));
            var content = new StringContent(updatedJson, Encoding.UTF8, "application/json");
            await client.PutAsync($"http://localhost:5194/api/GamesDB/update/{game.Id}", content);
        }
        else
        {
            Console.WriteLine("[WebController]: Попытка сделать ход вне своей очереди");
            return RedirectToPage("/Error", new { Code = 403 });
        }

        return RedirectToAction("GetGame", new { guid = game.Id });
    }

    [HttpGet("game/all")]
    public async Task<IActionResult> AllGames()
    {
        using var client = new HttpClient();
        var response = await client.GetAsync("http://localhost:5194/api/GamesDB/getList");

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var dtoList = JsonSerializer.Deserialize<List<GameDTO>>(json);

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

    [HttpPost("user/register")]
    public async Task<IActionResult> Register(string login, string password)
    {
        var signUpRequest = new SignUpRequest{
            Login = login,
            Password = password
        };

        var json = JsonSerializer.Serialize(signUpRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var client = new HttpClient();
        var response = await client.PostAsync("http://localhost:5194/api/users/register", content);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Login");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Ошибка регистрации");
            return View();
        }
    }

    [HttpGet("user/register")]
    public IActionResult Register()
    {
        return View();
    }

    [HttpGet("user/login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost("user/login")]
    public async Task<IActionResult> Login(string login, string password)
    {
        var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}"));
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);

        var response = await client.PostAsync("https://localhost:5194/api/users/authorize", null);
        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Неверный логин или пароль");
            return View();
        }

        var userIdString = await response.Content.ReadAsStringAsync();
        if (!Guid.TryParse(userIdString, out var userId))
        {
            ModelState.AddModelError("", "Ошибка авторизации");
            return View();
        }

        var claims = new List<Claim> {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        new Claim(ClaimTypes.Name, login),
    };
        await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies")));
        return RedirectToAction("LoginSuccess", new { userId });
    }

    [HttpGet]
    public IActionResult LoginSuccess(Guid userId)
    {
        var model = new LoginSuccessModel { UserId = userId };
        return View(model);
    }

    [HttpPost("user/logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(); // Очищает куки
        return RedirectToAction("Index");
    }

    [HttpPost("JoinGame")]
    public async Task<IActionResult> JoinGame(Guid id)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdString, out var userId))
            return RedirectToPage("/Error", new { Code = 403 });

        using var client = new HttpClient();
        var content = new StringContent(JsonSerializer.Serialize(userId), Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"http://localhost:5194/api/GamesDB/join/{id}", content);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("GetGame", new { guid = id });
        }

        return RedirectToPage("/Error", new { Code = 400 });
    }
}
