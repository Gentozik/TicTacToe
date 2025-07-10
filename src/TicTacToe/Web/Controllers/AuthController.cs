using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using TicTacToe.Domain.Model;
using TicTacToe.Services;

namespace TicTacToe.Web.Controllers;

[Route("game")]
public class AuthController(IUserService userService) : Controller
{
    private readonly IUserService _userService = userService;

    [HttpPost("user/register")]
    public async Task<IActionResult> Register(string login, string password)
    {
        var signUpRequest = new SignUpRequest{
            Login = login,
            Password = password
        };

        var result = await userService.RegisterUserAsync(signUpRequest);

        if (result == true)
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
    public async Task<IActionResult> Login(string? returnUrl = null)
    {
        const string realm = "MyApp";

        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authHeader))
        {
            HttpContext.Response.StatusCode = 401;
            HttpContext.Response.Headers.Append("WWW-Authenticate", $"Basic realm=\"{realm}\"");
            return new EmptyResult();
        }
        var userIdJson = await _userService.AuthorizeUserAsync(authHeader);

        if (userIdJson == null)
        {
            HttpContext.Response.StatusCode = 401;
            HttpContext.Response.Headers.Append("WWW-Authenticate", $"Basic realm=\"{realm}\"");
            return new EmptyResult();
        }

        HttpContext.Session.SetString("UserId", userIdJson);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("LoginSuccess");
    }

    [HttpGet("user/loginsuccess")]
    public IActionResult LoginSuccess()
    {
        string? userIdString = HttpContext.Session.GetString("UserId");

        if (Guid.TryParse(userIdString, out Guid userId))
        {
            var model = new LoginSuccessModel(userId);
            return View(model);
        }

        return RedirectToAction("Login");
    }
}
