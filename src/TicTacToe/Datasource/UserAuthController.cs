using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicTacToe.Datasource.Model;
using TicTacToe.Domain.Model;
using TicTacToe.Services;

namespace TicTacToe.Datasource
{
    [ApiController]
    [Route("api/users")]
    public class UserAuthController : Controller
    {
        private readonly IUserService _userService;

        public UserAuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] SignUpRequest request)
        {
            var user = await _userService.RegisterUserAsync(request.Login, request.Password);
            return Ok();
        }

        [HttpPost("authorize")]
        public async Task<IActionResult> Authorize()
        {

            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Unauthorized();
            }

            //var user = await _userService.AuthorizeUserAsync(request.Login, request.Password);
            //if (user == null)
            //    return Unauthorized();
            //return Ok(user.Uuid);
            return Ok();
        }
    }
}
