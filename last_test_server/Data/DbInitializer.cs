using last_test_server.Models;
using Microsoft.AspNetCore.Identity;
namespace last_test_server.Data
{
    public class DbInitializer
    {
        public static async Task InitializeAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            string[] roleNames = { "Admin", "Seller", "Buyer" };
            foreach(var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            var adminEmail = "admin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName= adminEmail,
                    Email = adminEmail,
                    FirstName="Администратор",
                    LastName="Системы",
                    EmailConfirmed=true,
                    PhoneNumber="+7 (999) 999-99-99",
                    City="Москва",
                    RegistrationDate=DateTime.Now,
                    IsActive=true,

                };
                var createAdmin = await userManager.CreateAsync(adminUser, "Admin123!");
                if (createAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
            var sellerEmail = "seller@example.com";
            var sellerUser = await userManager.FindByEmailAsync(sellerEmail);
            if (sellerUser == null)
            {
                sellerUser = new ApplicationUser
                {
                    UserName = sellerEmail,
                    Email = sellerEmail,
                    FirstName="Seller",
                    LastName="Adidas",
                    EmailConfirmed=true,
                    PhoneNumber="+7 (800) 555-35-35",
                    City="Шанхай",
                    RegistrationDate=DateTime.Now,
                    IsActive=true,
                };
                var createSeller = await userManager.CreateAsync(sellerUser, "Seller123!");
                if (createSeller.Succeeded)
                {
                    await userManager.AddToRoleAsync(sellerUser, "Seller");
                }
            }

            for(int i = 1; i <= 3; i++)
            {
                var buyerEmail = $"buyer{i}@example.com";
                var buyerUser= await userManager.FindByEmailAsync(buyerEmail);
                if(buyerUser == null)
                {
                    buyerUser = new ApplicationUser
                    {
                        UserName=buyerEmail,
                        Email=buyerEmail,
                        FirstName= "Buyer",
                        LastName=$"{i}",
                        EmailConfirmed=true,
                        PhoneNumber=$"+7 (999) {i}11-22-33",
                        City="Саратов",
                        RegistrationDate=DateTime.Now.AddDays(-i),
                        IsActive=true,
                    };
                    var createBuyer = await userManager.CreateAsync(buyerUser, "Buyer123!");
                    if (createBuyer.Succeeded)
                    {
                        await userManager.AddToRoleAsync(buyerUser, "Buyer");
                    }
                }
            }
        }
    }
}
