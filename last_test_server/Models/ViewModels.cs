using System.ComponentModel.DataAnnotations;

namespace last_test_server.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный Email")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Имя обязательно")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Фамилия обязательна")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Телефон")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Город")]
        public string? City { get; set; }

        [Display(Name = "Принимаю условия")]
        public bool TermsAccepted { get; set; }
    }
    public class LoginViewModel
    {
        [Required(ErrorMessage ="Email обязателен")]
        [EmailAddress]
        [Display(Name ="Email")]
        public string Email { get; set; }=string.Empty;

        [Required(ErrorMessage ="Пароль обязателен")]
        [DataType(DataType.Password)]
        [Display(Name ="Пароль")]
        public string Password { get; set; }= string.Empty;

        [Display(Name ="Запомнить")]
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }
    public class EditProfileViewModel
    {
        public string UserId { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [Display(Name ="Имя")]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; } = string.Empty;
        [Phone]
        [Display(Name ="Телефон")]
        public string? PhoneNumber { get; set; }
        [Display(Name = "Адрес")]
        public string? Address { get; set; }
        [Display(Name = "Город")]
        public string? City { get; set; }
        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
    }
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Текущий пароль")]
        public string OldPassword { get; set; }= string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100,MinimumLength =6)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }=string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение")]
        [Compare("New Password",ErrorMessage ="Пароли не совпадают")]
        public string ConfirmPassword { get; set;} = string.Empty;

    }
    
}
