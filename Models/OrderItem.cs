namespace moon.Models
{
    public class OrderItem
    {
        public string ID { get; set; } 
        public string ProductId { get; set; }       // ID sản phẩm
        public string ProductName { get; set; }     // Tên sản phẩm tại thời điểm đặt hàng
        public decimal Price { get; set; }          // Giá sản phẩm tại thời điểm đặt
        public int Quantity { get; set; }           // Số lượng mua
    }
}
