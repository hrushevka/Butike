using System.ComponentModel.DataAnnotations;

namespace last_test_server.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public string? ShippingAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Comment { get; set; }

        // Навигационные свойства
        public virtual ApplicationUser? User { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int PriceAtTime { get; set; }

        // Навигационные свойства
        public virtual Order? Order { get; set; }
        public virtual Product? Product { get; set; }
    }

    public enum OrderStatus
    {
        [Display(Name = "Ожидает обработки")]
        Pending,
        [Display(Name = "Подтвержден")]
        Confirmed,
        [Display(Name = "В доставке")]
        Shipping,
        [Display(Name = "Доставлен")]
        Delivered,
        [Display(Name = "Отменен")]
        Cancelled
    }

    public class EditOrderStatusViewModel
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus CurrentStatus { get; set; }
        [Required(ErrorMessage = "Выберите статус заказа")]
        public OrderStatus SelectedStatus { get; set; }
        public List<StatusSelection> Statuses { get; set; } = new();
    }



    public class StatusSelection
    {
        public OrderStatus Status { get; set; }
        public string StatusName { get; set; }
        public bool IsSelected { get; set; }
    }
}