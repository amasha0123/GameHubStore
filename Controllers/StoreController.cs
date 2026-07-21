using GameHubStore.Data;
using GameHubStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GameHubStore.Controllers
{
    public class StoreController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StoreController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, int? genreId)
        {
            var games = _context.Games.Include(g => g.Genre).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                games = games.Where(g => g.Title.Contains(searchString));
            }

            if (genreId.HasValue)
            {
                games = games.Where(g => g.GenreId == genreId.Value);
            }

            ViewBag.Genres = new SelectList(await _context.Genres.ToListAsync(), "GenreId", "Name", genreId);
            ViewBag.CurrentSearch = searchString;

            return View(await games.OrderByDescending(g => g.ReleaseDate).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var game = await _context.Games
                .Include(g => g.Genre)
                .FirstOrDefaultAsync(m => m.GameId == id);

            if (game == null) return NotFound();

            return View(game);
        }
    }
}
