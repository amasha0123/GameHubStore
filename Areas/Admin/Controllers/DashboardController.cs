using GameHubStore.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GameHubStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize] // Ideally [Authorize(Roles = "Admin")] but we didn't setup roles yet
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Top level metrics
            ViewBag.TotalRevenue = await _context.Orders.SumAsync(o => o.Total);
            ViewBag.TotalOrders = await _context.Orders.CountAsync();
            ViewBag.TotalGames = await _context.Games.CountAsync();
            ViewBag.TotalUsers = await _context.Users.CountAsync();

            // Monthly Sales Data for Chart.js (Last 6 Months)
            var last6Months = Enumerable.Range(0, 6)
                .Select(i => DateTime.UtcNow.AddMonths(-i).ToString("MMM"))
                .Reverse()
                .ToList();

            var salesData = new decimal[6];
            for (int i = 0; i < 6; i++)
            {
                var monthDate = DateTime.UtcNow.AddMonths(-(5 - i));
                var revenue = await _context.Orders
                    .Where(o => o.OrderDate.Month == monthDate.Month && o.OrderDate.Year == monthDate.Year)
                    .SumAsync(o => o.Total);
                salesData[i] = revenue;
            }

            ViewBag.Months = last6Months;
            ViewBag.SalesData = salesData;

            // Top Games Data
            var topGames = await _context.OrderItems
                .Include(oi => oi.Game)
                .GroupBy(oi => oi.GameId)
                .Select(g => new {
                    Title = g.First().Game!.Title,
                    Count = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(g => g.Count)
                .Take(5)
                .ToListAsync();

            ViewBag.TopGamesTitles = topGames.Select(g => g.Title).ToList();
            ViewBag.TopGamesCounts = topGames.Select(g => g.Count).ToList();

            return View();
        }
    }
}
