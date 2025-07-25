using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using moon.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace moon.Controllers
{
    public class ManagerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManagerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index() => View("~/Views/Home/Manager/Index.cshtml");
        public IActionResult Category()
        {
            // Lấy danh sách Category và đếm số sản phẩm từng loại
            var categories = _context.Categories
                .Select(c => new
                {
                    Id = c.Id,
                    Name = c.Name,
                    ProductCount = _context.Products.Count(p => p.CategoryId == c.Id)
                })
                .ToList();

            ViewBag.Categories = categories;
            return View("~/Views/Home/Manager/Category.cshtml");
        }
        public IActionResult Invoice() => View("~/Views/Home/Manager/Invoice.cshtml");
        public IActionResult Statistical() => View("~/Views/Home/Manager/Statistical.cshtml");
        public IActionResult StatusOrder() => View("~/Views/Home/Manager/StatusOrder.cshtml");
        public IActionResult AddCategory() => View("~/Views/Home/Manager/AddCategory.cshtml");
        public IActionResult Profile() => View("~/Views/Home/Manager/Profile.cshtml");

        public IActionResult Product()
        {
            var products = _context.Products.Include(p => p.Category).ToList();
            return View("~/Views/Home/Manager/Product.cshtml", products);
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View("~/Views/Home/Manager/AddProduct.cshtml");
        }

        [HttpPost]
        public IActionResult AddProduct([FromForm] Product product, IFormFile Image)
        {
            ViewBag.Categories = _context.Categories.ToList();

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ, vui lòng kiểm tra lại.";
                return View("~/Views/Home/Manager/AddProduct.cshtml", product);
            }

            try
            {
                // ✅ Sinh mã SPxxx tự động
                var lastProduct = _context.Products
                    .Where(p => p.Id.StartsWith("SP"))
                    .OrderByDescending(p => p.Id)
                    .FirstOrDefault();

                int nextNumber = 1;
                if (lastProduct != null && int.TryParse(lastProduct.Id.Substring(2), out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }

                product.Id = $"SP{nextNumber:D3}";

                // ✅ Xử lý ảnh
                if (Image != null && Image.Length > 0)
                {
                    var fileName = Path.GetFileName(Image.FileName);
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var savePath = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        Image.CopyTo(stream);
                    }

                    var imageUrl = "/uploads/" + fileName;
                    product.ImageUrls = new List<string> { imageUrl };
                }
                else
                {
                    product.ImageUrls = new List<string>(); // 👈 thêm dòng này để tránh lỗi null
                }


                _context.Products.Add(product);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                return RedirectToAction("Product");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm sản phẩm: " + ex.Message;
                return View("~/Views/Home/Manager/AddProduct.cshtml", product);
            }
        }

        [HttpGet]
        public IActionResult ViewProductDetail(string id)
        {
            var product = _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy sản phẩm!";
                return RedirectToAction("Product");
            }

            return View("~/Views/Home/Manager/ProductDetails.cshtml", product); // ← CHÚ Ý tên đúng là ProductDetails
        }

        [HttpGet]
        public IActionResult EditProduct(string id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy sản phẩm!";
                return RedirectToAction("Product");
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View("~/Views/Home/Manager/EditProduct.cshtml", product);
        }

        [HttpPost]
        public IActionResult EditProduct([FromForm] Product updatedProduct, IFormFile Image)
        {
            var existingProduct = _context.Products.FirstOrDefault(p => p.Id == updatedProduct.Id);
            if (existingProduct == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy sản phẩm cần sửa!";
                return RedirectToAction("Product");
            }

            try
            {
                // Cập nhật các thuộc tính
                existingProduct.Name = updatedProduct.Name;
                existingProduct.Description = updatedProduct.Description;
                existingProduct.Price = updatedProduct.Price;
                existingProduct.StockQuantity = updatedProduct.StockQuantity;
                existingProduct.CategoryId = updatedProduct.CategoryId;

                // Nếu có ảnh mới
                if (Image != null && Image.Length > 0)
                {
                    var fileName = Path.GetFileName(Image.FileName);
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var savePath = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        Image.CopyTo(stream);
                    }

                    var imageUrl = "/uploads/" + fileName;
                    existingProduct.ImageUrls = new List<string> { imageUrl };
                }
                // ❌ Không cần else ở đây vì bạn giữ nguyên ảnh cũ nếu không upload mới


                _context.SaveChanges();

                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction("Product");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi cập nhật: " + ex.Message;
                ViewBag.Categories = _context.Categories.ToList();
                return View("~/Views/Home/Manager/EditProduct.cshtml", updatedProduct);
            }
        }
        [HttpPost]
        public IActionResult AddCategory(string CategoryName)
        {
            try
            {
                // Lấy loại sản phẩm cuối có mã bắt đầu bằng "PK"
                var lastCategory = _context.Categories
                    .Where(c => c.Id.StartsWith("PK"))
                    .OrderByDescending(c => c.Id)
                    .FirstOrDefault();

                int nextNumber = 1;
                if (lastCategory != null && int.TryParse(lastCategory.Id.Substring(2), out int lastNum))
                {
                    nextNumber = lastNum + 1;
                }

                var newCategory = new Category
                {
                    Id = $"PK{nextNumber:D3}",
                    Name = CategoryName
                };

                _context.Categories.Add(newCategory);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Thêm loại sản phẩm thành công!";
                return RedirectToAction("Category");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi thêm loại sản phẩm: " + ex.Message;
                return View("~/Views/Home/Manager/AddCategory.cshtml");
            }
        }
        public IActionResult DeleteCategory(string id)
        {
            var category = _context.Categories.Find(id);
            if (category != null)
            {
                // Kiểm tra nếu không có sản phẩm thuộc loại thì mới xóa
                var count = _context.Products.Count(p => p.CategoryId == id);
                if (count == 0)
                {
                    _context.Categories.Remove(category);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Xóa loại sản phẩm thành công.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể xóa loại sản phẩm đang có sản phẩm.";
                }
            }

            return RedirectToAction("Category");
        }


        public IActionResult DeleteProduct(string id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy sản phẩm!";
                return RedirectToAction("Product");
            }

            try
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Xóa sản phẩm thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi xóa sản phẩm: " + ex.Message;
            }

            return RedirectToAction("Product");
        }

    }


}
