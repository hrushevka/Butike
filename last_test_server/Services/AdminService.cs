using last_test_server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace last_test_server.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<UserWithRolesViewModel>> GetAllUsersAsync()
        {
            var users = await _userManager.Users
                .OrderByDescending(u => u.RegistrationDate)
                .ToListAsync();

            var result = new List<UserWithRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new UserWithRolesViewModel
                {
                    UserId = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = roles.ToList()
                });
            }

            return result;
        }

        public async Task<UserWithRolesViewModel> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("Пользователь не найден");

            var roles = await _userManager.GetRolesAsync(user);

            return new UserWithRolesViewModel
            {
                UserId = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles.ToList()
            };
        }

        public async Task<bool> UpdateUserRolesAsync(string userId, List<string> roles)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return false;

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return false;

                var userRoles = await _userManager.GetRolesAsync(user);

                if (userRoles != null && userRoles.Any())
                {
                    var rolesToRemove = userRoles.ToList();
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

                    if (!removeResult.Succeeded)
                        return false;
                }

                if (roles != null && roles.Any())
                {
                    var rolesToAdd = roles.Where(r => !string.IsNullOrEmpty(r)).ToList();

                    if (rolesToAdd.Any())
                    {
                        var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                        return addResult.Succeeded;
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}