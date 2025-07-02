using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using TicTacToe.Domain.Model;
using TicTacToe.Services;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService) => _userService = userService;

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] SignUpRequest request)
    {
        var user = await _userService.RegisterUserAsync(request.Login, request.Password);
        return Ok(user.Uuid);
    }

    [AllowAnonymous]
    [HttpPost("authorize")]
    public async Task<IActionResult> Authorize()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            return Unauthorized();

        var encoded = authHeader.ToString().Replace("Basic ", "", StringComparison.OrdinalIgnoreCase).Trim();
        var decodedBytes = Convert.FromBase64String(encoded);
        var decodedString = Encoding.UTF8.GetString(decodedBytes);

        var parts = decodedString.Split(':', 2);
        if (parts.Length != 2)
            return Unauthorized();

        var login = parts[0];
        var password = parts[1];

        var user = await _userService.AuthorizeUserAsync(login, password);
        if (user == null)
            return Unauthorized();

        return Ok(user.Uuid);
    }
}
