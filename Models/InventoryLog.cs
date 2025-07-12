namespace moon.Models
{
    public class InventoryLog
    {
        public string Id { get; set; }                   // ID log
        public string ProductId { get; set; }            // Sản phẩm bị thay đổi tồn kho
        public int Change { get; set; }                  // Thay đổi: +10, -5, ...
        public string Reason { get; set; }               // Lý do thay đổi
        public DateTime Timestamp { get; set; }          // Thời điểm thay đổi
    }
}
