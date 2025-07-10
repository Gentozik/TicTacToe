using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicTacToe.Datasource.Model;
using TicTacToe.Domain.Model;

namespace TicTacToe.Datasource
{
    [ApiController]
    [Route("api/GamesDB")]
    public class GamesDBController : Controller
    {
        private readonly AppDbContext _context;

        public GamesDBController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("getList")]
        public async Task<ActionResult<IEnumerable<GameDTO>>> GetAllGames()
        {
            IQueryable<GameDTO> query = _context.Games;

            query = query.Where(g => g.GameOutcome == GameOutcome.WaitingForPlayers);

            var games = await query.ToListAsync();

            return games;
        }


        [HttpGet("get/{id}")]
        public async Task<ActionResult<GameDTO>> GetGame(Guid id)
        {
            var game = await _context.Games.FindAsync(id);

            if (game == null)
                return NotFound();

            return Ok(game);
        }

        public async Task<GameDTO?> SaveGame(Guid id)
        {
            GameDTO? game = await _context.Games.FindAsync(id);

            return game;
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateGame(Guid id, GameDTO updatedGame)
        {
            Console.WriteLine("[GamesDBController]: Попытка обновления игры");

            if (id != updatedGame.Id)
                return BadRequest();

            var existingGame = await _context.Games.FindAsync(id);

            if (existingGame == null)
                return NotFound();

            _context.Games.Remove(existingGame);

            existingGame = updatedGame;

            try
            {
                _context.Games.Add(existingGame);
                await _context.SaveChangesAsync();
                Console.WriteLine("[GamesDBController]: Сохранение успешно");
            }
            catch (DbUpdateConcurrencyException)
            {
                Console.WriteLine("[GamesDBController]: Ошибка сохранения");
                if (!await GameExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpPost("join/{id:guid}")]
        public async Task<ActionResult<GameDTO>> JoinGame(Guid id, [FromBody] Guid playerId)
        {
            var game = await _context.Games.FindAsync(id);

            if (game == null)
                return NotFound();

            if (game.PlayerOId != null && game.PlayerOId != Guid.Empty)
                return BadRequest("Игра уже заполнена.");

            game.PlayerOId = playerId;
            game.GameOutcome = GameOutcome.FirstPlayerTurn;
            await _context.SaveChangesAsync();

            return Ok(game);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteGame(Guid id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
                return NotFound();

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private Task<bool> GameExists(Guid id) =>
            _context.Games.AnyAsync(g => g.Id == id);
    }
}
