using System.ComponentModel.DataAnnotations;

namespace last_test_server.Models
{
    public class CheckoutViewModel
    {
        public decimal TotalPrice { get; set; }

        [Display(Name = "Имя")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите номер телефона")]
        [Phone(ErrorMessage = "Неверный формат телефона")]
        [Display(Name = "Телефон")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите адрес доставки")]
        [Display(Name = "Адрес доставки")]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Комментарий к заказу")]
        [StringLength(500)]
        public string? Comment { get; set; }
    }
}