using Microsoft.AspNetCore.Mvc;
using moon.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace moon.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult AddToCart(string id, int quantity = 1, string returnUrl = null)
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            var cartItem = _context.CartItems.FirstOrDefault(c =>
                c.UserId == user.Id && c.ProductId == product.Id);

            int currentQuantityInCart = cartItem?.Quantity ?? 0;

            // Tổng số lượng sau khi thêm không được vượt quá tồn kho
            if (currentQuantityInCart + quantity > product.StockQuantity)
            {
                TempData["Error"] = $"Bạn đã có {currentQuantityInCart} sản phẩm trong giỏ hàng. Không thể thêm vì vượt quá tồn kho.";

                // Nếu có returnUrl thì quay lại đúng URL cũ (ví dụ: trang chi tiết sản phẩm)
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                // Nếu không có thì quay lại chính trang chi tiết sản phẩm
                return RedirectToAction("ProductDetail", "Customer", new { id = product.Id });
            }

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                var newItem = new CartItem
                {
                    Id = GenerateNewCartItemId(),
                    UserId = user.Id,
                    ProductId = product.Id,
                    ImageUrl = product.ImageUrls?.FirstOrDefault() ?? "/images/default.png",
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = quantity,
                    StockQuantity = product.StockQuantity
                };

                _context.CartItems.Add(newItem);
            }

            _context.SaveChanges();
            TempData["Success"] = "Đã thêm sản phẩm vào giỏ hàng!";

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Customer", "Home");
        }

        private string GenerateNewCartItemId()
        {
            var lastItem = _context.CartItems
                .OrderByDescending(c => c.Id)
                .FirstOrDefault();

            if (lastItem == null || string.IsNullOrEmpty(lastItem.Id))
                return "GH001";

            var numberPart = int.Parse(lastItem.Id.Substring(2));
            var newNumber = numberPart + 1;
            return $"GH{newNumber:D3}";
        }

        [HttpGet]
        public IActionResult Index()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var cartItems = _context.CartItems
     .Where(c => c.UserId == user.Id)
     .ToList();

            foreach (var item in cartItems)
            {
                var product = _context.Products.FirstOrDefault(p => p.Id == item.ProductId);
                item.StockQuantity = product?.StockQuantity ?? 0;
            }

            return View("~/Views/Home/Customer/Cart.cshtml", cartItems);

        }
        [HttpGet]
        public IActionResult UpdateQuantity(string id, int quantity)
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null) return Unauthorized();

            var cartItem = _context.CartItems.FirstOrDefault(c => c.Id == id && c.UserId == user.Id);
            if (cartItem == null) return NotFound();

            var product = _context.Products.FirstOrDefault(p => p.Id == cartItem.ProductId);
            if (product == null) return NotFound();

            // Giới hạn không vượt quá tồn kho
            if (quantity > product.StockQuantity)
            {
                quantity = product.StockQuantity;
            }

            if (quantity < 1)
            {
                quantity = 1;
            }

            cartItem.Quantity = quantity;
            _context.SaveChanges();

            return Ok();
        }
        [HttpGet]
        public IActionResult Remove(string id)
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email)) return RedirectToAction("Login", "Login");

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null) return RedirectToAction("Login", "Login");

            var item = _context.CartItems.FirstOrDefault(c => c.Id == id && c.UserId == user.Id);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        public int GetCartItemCount()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email)) return 0;

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null) return 0;

            // Đếm số sản phẩm khác nhau trong giỏ hàng
            return _context.CartItems.Count(c => c.UserId == user.Id);
        }

    }
}
