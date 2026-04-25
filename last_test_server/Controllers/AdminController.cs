using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using last_test_server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using last_test_server.Data;

namespace last_test_server.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }


        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var usersWithRoles = new List<UserWithRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                usersWithRoles.Add(new UserWithRolesViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = roles.ToList()
                });
            }

            return View(usersWithRoles);
        }

        [HttpGet]
        public async Task<IActionResult> EditRoles(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = new List<string> { "Admin", "Seller", "Buyer" };

            var model = new EditUserRoleViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                Roles = allRoles.Select(r => new RoleSelection
                {
                    RoleName = r,
                    IsSelected = userRoles.Contains(r)
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoles(EditUserRoleViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            var selectedRoles = model.Roles
                .Where(r => r.IsSelected)
                .Select(r => r.RoleName)
                .ToList();

            var currentRoles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (selectedRoles.Any())
            {
                await _userManager.AddToRolesAsync(user, selectedRoles);
            }

            TempData["SuccessMessage"] = "Роли пользователя обновлены!";
            return RedirectToAction("Users");
        }


        public async Task<IActionResult> Orders()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> EditOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            var allStatuses = new List<OrderStatus>
            {
                OrderStatus.Pending,
                OrderStatus.Confirmed,
                OrderStatus.Shipping,
                OrderStatus.Delivered,
                OrderStatus.Cancelled
            };

            var model = new EditOrderStatusViewModel
            {
                OrderId = order.Id,
                OrderNumber = order.Id.ToString(),
                CustomerName = order.User != null ? $"{order.User.FirstName} {order.User.LastName}" : order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                CurrentStatus = order.Status,
                Statuses = allStatuses.Select(s => new StatusSelection
                {
                    Status = s,
                    StatusName = GetStatusName(s),
                    IsSelected = s == order.Status
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOrder(EditOrderStatusViewModel model)
        {
            var order = await _context.Orders.FindAsync(model.OrderId);
            if (order == null) return NotFound();

            var selectedStatus = model.Statuses
                .Where(s => s.IsSelected)
                .Select(s => s.Status)
                .FirstOrDefault();

            order.Status = selectedStatus;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Статус заказа #{order.Id} обновлен на \"{GetStatusName(selectedStatus)}\"!";
            return RedirectToAction("Orders");
        }

        private string GetStatusName(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "Ожидает обработки",
                OrderStatus.Confirmed => "Подтвержден",
                OrderStatus.Shipping => "В доставке",
                OrderStatus.Delivered => "Доставлен",
                OrderStatus.Cancelled => "Отменен",
                _ => status.ToString()
            };
        }
    }
}