using System.Diagnostics;
using last_test_server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using last_test_server.Data;

namespace last_test_server.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Seller)
                .Where(p => p.IsActive) // Если добавите поле IsActive
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            // Получаем корзину текущего пользователя (если авторизован)
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var cartItems = await _context.CartItems
                        .Where(c => c.UserId == user.Id)
                        .ToListAsync();
                    ViewBag.CartCount = cartItems.Sum(c => c.Quantity);
                }
            }

            return View(products);
        }
    }
}