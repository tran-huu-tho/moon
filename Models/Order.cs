using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace moon.Models
{
    public class Order
    {
        public string Id { get; set; }                  // ID đơn hàng
        public string UserId { get; set; }              // ID người dùng đặt hàng

        [NotMapped] // EF không lưu vào bảng Order, chỉ dùng để truy vấn nếu cần
        public List<OrderItem>? Items { get; set; }
        
        public decimal Total { get; set; }              // Tổng tiền
        public string Status { get; set; }              // Trạng thái đơn
        public string ShippingAddress { get; set; }     // Địa chỉ giao hàng
        public string? Note { get; set; }               // Ghi chú (nếu có)
        public DateTime OrderDate { get; set; }         // Ngày đặt hàng
    }
}
