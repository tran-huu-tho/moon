using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace moon.Models
{
    public class Order
    {
        [Key]
        public string Id { get; set; } 

        public string UserId { get; set; }               // Liên kết tới người dùng

        [Required]
        public string FullName { get; set; }             // Họ tên người nhận

        [Required]
        public string PhoneNumber { get; set; }          // Số điện thoại

        [Required]
        public string Province { get; set; }             // Tỉnh/TP

        [Required]
        public string District { get; set; }             // Quận/Huyện

        [Required]
        public string Ward { get; set; }                 // Phường/Xã

        public string Note { get; set; }                 // Ghi chú

        public string PaymentMethod { get; set; }        // COD, chuyển khoản, etc.

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public decimal ShippingFee { get; set; } = 30000;

        public decimal TotalAmount { get; set; }         // Tổng cộng (gồm cả phí ship)

        // Navigation property
        public List<OrderItem> Items { get; set; }
    }
}