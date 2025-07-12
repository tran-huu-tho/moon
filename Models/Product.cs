using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace moon.Models
{
    public class Product
    {
        public string Id { get; set; }                    // ID sản phẩm
        public string Name { get; set; }                  // Tên sản phẩm
        public string Description { get; set; }           // Mô tả
        public decimal Price { get; set; }                // Giá bán
        public int StockQuantity { get; set; }            // Số lượng trong kho
        public string CategoryId { get; set; }            // ID danh mục
        public string ImageUrlsJson { get; set; }         // Trường thật để lưu trong DB

        [NotMapped]
        public List<string> ImageUrls                     // Trường ảo chỉ dùng trong code
        {
            get => string.IsNullOrEmpty(ImageUrlsJson)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(ImageUrlsJson);
            set => ImageUrlsJson = JsonSerializer.Serialize(value);
        }
    }
}
