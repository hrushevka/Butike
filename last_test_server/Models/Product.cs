using System.ComponentModel.DataAnnotations;

namespace last_test_server.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Название обязательно")]
        [Display(Name = "Название")]
        [StringLength(50, ErrorMessage = "Название не может быть длиннее 50 символов")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ссылка на картинку обязательна")]
        [Display(Name = "Ссылка на картинку")]
        [StringLength(450)]
        public string ImageURL { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Цена")]
        [Range(0, 1000000, ErrorMessage = "Цена должна быть от 0 до 1,000,000")]
        public int Cost { get; set; }

        [Required(ErrorMessage = "ID продавца обязателен")]
        [Display(Name = "ID продавца")]
        public string SellerId { get; set; } = string.Empty;

        [Display(Name = "Состав")]
        [StringLength(500)]
        public string? Composition { get; set; }

        [Display(Name = "Дата добавления")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Навигационное свойство
        public virtual ApplicationUser? Seller { get; set; }
        [Display(Name = "Активен")]
        public bool IsActive { get; set; } = true;
    }
}