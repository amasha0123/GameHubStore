using GameHubStore.Data;
using GameHubStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GameHubStore.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var cartItems = await _context.Carts
                .Include(c => c.Game)
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            ViewBag.CartTotal = cartItems.Sum(c => c.Quantity * (c.Game?.Price ?? 0));

            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var game = await _context.Games.FindAsync(id);
            if (game == null) return NotFound();

            var existingCartItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == user.Id && c.GameId == id);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += 1;
                _context.Carts.Update(existingCartItem);
            }
            else
            {
                var cartItem = new Cart
                {
                    UserId = user.Id,
                    GameId = id,
                    Quantity = 1
                };
                _context.Carts.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartId)
        {
            var cartItem = await _context.Carts.FindAsync(cartId);
            if (cartItem != null)
            {
                _context.Carts.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartId, int quantity)
        {
            if (quantity <= 0)
            {
                return await RemoveFromCart(cartId);
            }

            var cartItem = await _context.Carts.FindAsync(cartId);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                _context.Carts.Update(cartItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
