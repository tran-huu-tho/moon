namespace moon.Models
{
    public class Invoice
    {
        public string Id { get; set; }                  // ID hóa đơn
        public string OrderId { get; set; }             // Liên kết tới đơn hàng
        public string UserId { get; set; }              // Người mua
        public decimal Total { get; set; }              // Tổng tiền thanh toán
        public DateTime IssuedDate { get; set; }        // Ngày phát hành hóa đơn
    }
}
