using System.ComponentModel.DataAnnotations;

namespace last_test_server.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 999)]
        public int Quantity { get; set; } = 1;

        [Required]
        public DateTime AddedAt { get; set; } = DateTime.Now;

        // Навигационные свойства
        public virtual ApplicationUser? User { get; set; }
        public virtual Product? Product { get; set; }
    }
}