using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TicTacToe.Pages.Shared
{
    public class GetGameModel : PageModel
    {
        private readonly ILogger<GetGameModel> _logger;

        public GetGameModel(ILogger<GetGameModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
