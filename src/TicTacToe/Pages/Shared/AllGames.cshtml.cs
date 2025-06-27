using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TicTacToe.Pages.Shared
{
    public class AllGamesModel : PageModel
    {
        private readonly ILogger<AllGamesModel> _logger;

        public AllGamesModel(ILogger<AllGamesModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
