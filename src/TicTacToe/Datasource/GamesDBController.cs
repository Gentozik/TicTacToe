using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicTacToe.Datasource.Model;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameDTO>>> GetAllGames()
        {
            return await _context.Games.ToListAsync();
        }

        // GET: api/games/get/{id}
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

        [HttpPost("create")]
        public async Task<ActionResult<GameDTO>> CreateGame()
        {
            Console.WriteLine("Попытка создать игру в контроллере");
            var newGame = new GameDTO();

            _context.Games.Add(newGame);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGame), new { id = newGame.Id }, newGame);
        }

        // PUT: api/games/update/{id}
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateGame(Guid id, GameDTO updatedGame)
        {
            if (id != updatedGame.Id)
                return BadRequest();

            _context.Entry(updatedGame).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await GameExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/games/{id}
        [HttpDelete("{id}")]
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
