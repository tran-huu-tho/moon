using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace moon.Models
{
    [Table("CartItems")]
    public class CartItem
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string ProductId { get; set; }

        public string ProductName { get; set; }

        public string ImageUrl { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string UserId { get; set; }

        [NotMapped]
        public decimal Total => Price * Quantity; 
        [NotMapped]
        public int StockQuantity { get; set; }

    }
}
