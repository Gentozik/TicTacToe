using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TicTacToe.Pages.Shared
{
    public class RegisterModel : PageModel
    {
        private readonly ILogger<LoginModel> _logger;

        public RegisterModel(ILogger<LoginModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
