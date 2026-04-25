using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using last_test_server.Data;
using last_test_server.Models;

namespace last_test_server.Controllers
{
    [Authorize(Roles = "Buyer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CartController> _logger;

        public CartController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<CartController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // Просмотр корзины
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            var totalPrice = cartItems.Sum(c => c.Product!.Cost * c.Quantity);
            ViewBag.TotalPrice = totalPrice;

            return View(cartItems);
        }

        // Добавление товара в корзину
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "Пользователь не авторизован" });

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return Json(new { success = false, message = "Товар не найден" });

            // Проверяем, есть ли уже такой товар в корзине
            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == user.Id && c.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                _context.Update(existingItem);
            }
            else
            {
                var cartItem = new CartItem
                {
                    UserId = user.Id,
                    ProductId = productId,
                    Quantity = quantity
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Товар добавлен в корзину" });
        }

        // Обновление количества товара
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "Пользователь не авторизован" });

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == user.Id);

            if (cartItem == null)
                return Json(new { success = false, message = "Товар не найден в корзине" });

            if (quantity <= 0)
            {
                _context.CartItems.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = quantity;
                _context.Update(cartItem);
            }

            await _context.SaveChangesAsync();

            // Пересчитываем общую сумму
            var totalPrice = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == user.Id)
                .SumAsync(c => c.Product!.Cost * c.Quantity);

            return Json(new { success = true, totalPrice = totalPrice });
        }

        // Удаление товара из корзины
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "Пользователь не авторизован" });

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == user.Id);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Товар удален из корзины" });
            }

            return Json(new { success = false, message = "Товар не найден" });
        }

        // Очистка всей корзины
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "Пользователь не авторизован" });

            var cartItems = _context.CartItems.Where(c => c.UserId == user.Id);
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Корзина очищена" });
        }

        // Оформление заказа
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "Ваша корзина пуста";
                return RedirectToAction(nameof(Index));
            }

            var totalPrice = cartItems.Sum(c => c.Product!.Cost * c.Quantity);

            var model = new CheckoutViewModel
            {
                TotalPrice = totalPrice,
                UserName = $"{user.FirstName} {user.LastName}",
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address
            };

            return View(model);
        }

        // Подтверждение заказа
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "Ваша корзина пуста";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                // Создаем заказ
                var order = new Order
                {
                    UserId = user.Id,
                    OrderDate = DateTime.Now,
                    TotalAmount = cartItems.Sum(c => c.Product!.Cost * c.Quantity),
                    Status = OrderStatus.Pending,
                    ShippingAddress = model.Address,
                    PhoneNumber = user.PhoneNumber,
                    Comment = model.Comment
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Добавляем элементы заказа
                foreach (var item in cartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        ProductName = item.Product!.Name,
                        Quantity = item.Quantity,
                        PriceAtTime = item.Product.Cost
                    };
                    _context.OrderItems.Add(orderItem);
                }

                // Очищаем корзину
                _context.CartItems.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Заказ #{order.Id} успешно оформлен!";
                return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
            }

            model.TotalPrice = cartItems.Sum(c => c.Product!.Cost * c.Quantity);
            return View(model);
        }

        // Подтверждение заказа
        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == user.Id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // История заказов
        public async Task<IActionResult> OrderHistory()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }
        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            if (!User.Identity.IsAuthenticated)
                return Ok(0);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Ok(0);

            var count = await _context.CartItems
                .Where(c => c.UserId == user.Id)
                .SumAsync(c => c.Quantity);

            return Ok(count);
        }
    }
}