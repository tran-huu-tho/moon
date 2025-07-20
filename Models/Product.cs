using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace moon.Models
{
    public class Product
    {
        
        public string Id { get; set; } = Guid.NewGuid().ToString();     // Mã sản phẩm

        public string Name { get; set; } = string.Empty;                // Tên sản phẩm

        public string Description { get; set; } = string.Empty;         // Mô tả

        public decimal Price { get; set; }                              // Giá

        public int StockQuantity { get; set; }                          // Tồn kho

        public string CategoryId { get; set; } = string.Empty;          // FK loại sản phẩm

        public string ImageUrlsJson { get; set; } = "[]";               // JSON danh sách ảnh

        [NotMapped]
        public List<string> ImageUrls
        {
            get => string.IsNullOrEmpty(ImageUrlsJson)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(ImageUrlsJson)!;

            set => ImageUrlsJson = JsonSerializer.Serialize(value);
        }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }                         // Quan hệ FK
    }
}
