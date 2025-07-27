using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace moon.Models
{
    public class OrderItem
    {
        [Key]
        public string Id { get; set; }

        public string OrderId { get; set; }

        public string ProductId { get; set; }
        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal Total { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }
    }

}
