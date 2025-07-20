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
        public IActionResult Category() => View("~/Views/Home/Manager/Category.cshtml");
        public IActionResult Invoice() => View("~/Views/Home/Manager/Invoice.cshtml");
        public IActionResult Statistical() => View("~/Views/Home/Manager/Statistical.cshtml");
        public IActionResult StatusOrder() => View("~/Views/Home/Manager/StatusOrder.cshtml");
        public IActionResult AddCategory() => View("~/Views/Home/Manager/AddCategory.cshtml");

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

    }
}
