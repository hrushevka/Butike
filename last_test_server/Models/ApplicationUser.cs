using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace last_test_server.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        [Display(Name = "Имя")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name ="Фамилия")]
        [StringLength(50)]
        public string LastName { get; set; }= string.Empty;

        [Display(Name = "Полное имя")]
        public string FullName => $"{FirstName} {LastName}";

        [DataType(DataType.Date)]
        [Display(Name ="Дата регистрации")]
        public DateTime RegistrationDate { get; set; }

        [Display(Name = "Активен")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Адрес")]
        [StringLength(200)]
        public string? Address { get; set; }

        [Display(Name ="Город")]
        [StringLength(50)]
        public string? City { get; set; }

        [Display(Name ="Дата рождения")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
       
        [Display(Name ="Последний вход")]
        public DateTime? LastLoginDate { get; set; }
        //[Display(Name ="Бутики")]
        //public List<CartModel> cartModels { get; set; } = new List<CartModel>();
    }
}
