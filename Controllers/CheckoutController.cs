using GameHubStore.Data;
using GameHubStore.Models;
using GameHubStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameHubStore.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckoutController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
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

            if (!cartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            ViewBag.CartItems = cartItems;
            ViewBag.Total = cartItems.Sum(c => c.Quantity * (c.Game?.Price ?? 0));

            return View(new CheckoutViewModel { FullName = user.Name ?? user.UserName, Email = user.Email });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessOrder(CheckoutViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var cartItems = await _context.Carts
                .Include(c => c.Game)
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            if (ModelState.IsValid)
            {
                var domain = "https://localhost:7196"; // Replace with dynamic domain in production
                
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    SuccessUrl = domain + "/Checkout/Success?session_id={CHECKOUT_SESSION_ID}",
                    CancelUrl = domain + "/Checkout/Index",
                    CustomerEmail = user.Email
                };

                foreach(var item in cartItems)
                {
                    options.LineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Game!.Price * 100), // Stripe expects cents
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Game.Title,
                            },
                        },
                        Quantity = item.Quantity,
                    });
                }

                var service = new SessionService();
                var session = service.Create(options);

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }

            ViewBag.CartItems = cartItems;
            ViewBag.Total = cartItems.Sum(c => c.Quantity * (c.Game?.Price ?? 0));
            return View("Index", model);
        }

        public async Task<IActionResult> Success(string session_id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var cartItems = await _context.Carts
                .Include(c => c.Game)
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            if (!cartItems.Any()) return RedirectToAction("Index", "Home");

            // Create the Order
            var order = new Order
            {
                UserId = user.Id,
                Total = cartItems.Sum(c => c.Quantity * (c.Game?.Price ?? 0)),
                PaymentMethod = "Stripe",
                Status = "Completed"
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Create Order Items
            foreach (var item in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    GameId = item.GameId,
                    Price = item.Game?.Price ?? 0,
                    Quantity = item.Quantity
                };
                _context.OrderItems.Add(orderItem);
            }

            // Clear the Cart
            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            // Note: Sending an email confirmation would happen here.

            return RedirectToAction(nameof(Invoice), new { id = order.OrderId });
        }

        public async Task<IActionResult> Invoice(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Game)
                .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == user.Id);

            if (order == null) return NotFound();

            return View(order);
        }
    }
}
