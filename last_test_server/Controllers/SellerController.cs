using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using last_test_server.Data;
using last_test_server.Models;

namespace last_test_server.Controllers
{
    [Authorize(Roles = "Seller")]
    public class SellerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SellerController> _logger;

        public SellerController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<SellerController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // Главная страница продавца со списком товаров
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var products = await _context.Products
                .Where(p => p.SellerId == user.Id)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            ViewBag.SellerName = $"{user.FirstName} {user.LastName}";
            return View(products);
        }

        // GET: Добавление товара
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Product());
        }

        // POST: Добавление товара
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            ModelState.Remove("SellerId");
            ModelState.Remove("Id");
            ModelState.Remove("Seller");

            if (ModelState.IsValid)
            {
                try
                {
                    model.SellerId = user.Id;
                    model.CreatedAt = DateTime.Now;
                    _context.Products.Add(model);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Товар успешно добавлен!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при добавлении товара");
                    ModelState.AddModelError(string.Empty, "Ошибка при добавлении товара");
                }
            }
            return View(model);
        }

        // GET: Редактирование товара
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.SellerId == user.Id);

            if (product == null)
            {
                TempData["ErrorMessage"] = "Товар не найден";
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        // POST: Редактирование товара
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            if (id != model.Id)
            {
                TempData["ErrorMessage"] = "Ошибка идентификации товара";
                return RedirectToAction(nameof(Index));
            }

            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.SellerId == user.Id);

            if (existingProduct == null)
            {
                TempData["ErrorMessage"] = "Товар не найден или у вас нет прав на его редактирование";
                return RedirectToAction(nameof(Index));
            }

            ModelState.Remove("SellerId");
            ModelState.Remove("Seller");

            if (ModelState.IsValid)
            {
                try
                {
                    existingProduct.Name = model.Name;
                    existingProduct.ImageURL = model.ImageURL;
                    existingProduct.Cost = model.Cost;
                    existingProduct.Composition = model.Composition;

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Товар успешно обновлен!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при обновлении товара");
                    ModelState.AddModelError(string.Empty, "Ошибка при обновлении товара");
                }
            }
            return View(model);
        }

        // GET: Удаление товара
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.SellerId == user.Id);

            if (product == null)
            {
                TempData["ErrorMessage"] = "Товар не найден";
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        // POST: Удаление товара
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.SellerId == user.Id);

            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Товар успешно удален!";
            }
            else
            {
                TempData["ErrorMessage"] = "Товар не найден";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}