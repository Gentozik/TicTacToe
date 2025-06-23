using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TicTacToe.Web.Model;

public class ErrorModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int Code { get; set; } = 500;

    public string Description { get; set; } = "Something went wrong.";

    public void OnGet()
    {
        Description = Code switch
        {
            404 => "Not found:\nThe page you're looking for doesn't exist.",
            400 => "Bad request: you're trying to perform an illegal action.",
            _ => "Something went wrong."
        };
    }
}