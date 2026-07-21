using GameHubStore.Data;
using GameHubStore.Models;
using GameHubStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GameHubStore.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var model = new UserProfileViewModel
            {
                User = user,
                Orders = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Game)
                    .Where(o => o.UserId == user.Id)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync(),
                Wishlists = await _context.Wishlists
                    .Include(w => w.Game)
                    .Where(w => w.UserId == user.Id)
                    .ToListAsync(),
                Reviews = await _context.Reviews
                    .Include(r => r.Game)
                    .Where(r => r.UserId == user.Id)
                    .OrderByDescending(r => r.Date)
                    .ToListAsync()
            };

            return View(model);
        }
    }
}
