using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using TicTacToe.Services;

public class AuthFilter : IAsyncActionFilter
{
    private readonly IUserService _userService;

    public AuthFilter(IUserService userService) => _userService = userService;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var encoded = authHeader.ToString().Replace("Basic ", "", StringComparison.OrdinalIgnoreCase).Trim();
        string decodedString;
        try
        {
            var decodedBytes = Convert.FromBase64String(encoded);
            decodedString = Encoding.UTF8.GetString(decodedBytes);
        }
        catch
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var parts = decodedString.Split(':', 2);
        if (parts.Length != 2)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var user = await _userService.AuthorizeUserAsync(parts[0], parts[1]);
        if (user == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Авторизация успешна — добавим userId в HttpContext.Items
        context.HttpContext.Items["UserId"] = user.Uuid;
        await next();
    }
}