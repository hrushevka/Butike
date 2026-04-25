using last_test_server.Models;

namespace last_test_server.Services
{
    public interface IAdminService
    {
        Task<List<UserWithRolesViewModel>> GetAllUsersAsync();
        Task<UserWithRolesViewModel> GetUserByIdAsync(string userId);
        Task<bool> UpdateUserRolesAsync(string userId, List<string> roles);
    }
}