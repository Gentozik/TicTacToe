using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class AuthFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;
        string? userId = session.GetString("UserId");

        if (string.IsNullOrEmpty(userId))
        {
            var response = context.HttpContext.Response;
            response.StatusCode = 401;
            response.Headers.Append("WWW-Authenticate", "Basic realm=\"MyApp\"");

            context.Result = new EmptyResult();
        } else
        {
            context.HttpContext.Items["UserId"] = userId;
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {

    }
}