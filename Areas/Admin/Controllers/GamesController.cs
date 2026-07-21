using GameHubStore.Areas.Admin.ViewModels;
using GameHubStore.Data;
using GameHubStore.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GameHubStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GamesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public GamesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            var games = _context.Games.Include(g => g.Genre).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                games = games.Where(g => g.Title.Contains(searchString));
            }

            // A simple approach for now, real world should use a Pagination List
            ViewData["CurrentFilter"] = searchString;
            return View(await games.OrderByDescending(g => g.GameId).ToListAsync());
        }

        public IActionResult Create()
        {
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Name");
            return View(new GameFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GameFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedFile(model);

                var game = new Game
                {
                    Title = model.Title,
                    Description = model.Description,
                    Price = model.Price,
                    Discount = model.Discount,
                    Developer = model.Developer,
                    Publisher = model.Publisher,
                    ReleaseDate = model.ReleaseDate,
                    GenreId = model.GenreId,
                    ImageUrl = uniqueFileName ?? "default-game.jpg"
                };

                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Name", model.GenreId);
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var game = await _context.Games.FindAsync(id);
            if (game == null) return NotFound();

            var model = new GameFormViewModel
            {
                GameId = game.GameId,
                Title = game.Title,
                Description = game.Description,
                Price = game.Price,
                Discount = game.Discount,
                Developer = game.Developer,
                Publisher = game.Publisher,
                ReleaseDate = game.ReleaseDate,
                GenreId = game.GenreId,
                ExistingImageUrl = game.ImageUrl
            };

            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Name", game.GenreId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GameFormViewModel model)
        {
            if (id != model.GameId) return NotFound();

            if (ModelState.IsValid)
            {
                var game = await _context.Games.FindAsync(id);
                if (game == null) return NotFound();

                game.Title = model.Title;
                game.Description = model.Description;
                game.Price = model.Price;
                game.Discount = model.Discount;
                game.Developer = model.Developer;
                game.Publisher = model.Publisher;
                game.ReleaseDate = model.ReleaseDate;
                game.GenreId = model.GenreId;

                if (model.ImageFile != null)
                {
                    if (game.ImageUrl != "default-game.jpg" && game.ImageUrl != null)
                    {
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "games", game.ImageUrl);
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                    game.ImageUrl = ProcessUploadedFile(model)!;
                }

                _context.Update(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Name", model.GenreId);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game != null)
            {
                if (game.ImageUrl != "default-game.jpg" && game.ImageUrl != null)
                {
                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "games", game.ImageUrl);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                _context.Games.Remove(game);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private string? ProcessUploadedFile(GameFormViewModel model)
        {
            string? uniqueFileName = null;
            if (model.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "games");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImageFile.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
