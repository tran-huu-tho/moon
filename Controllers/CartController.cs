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
            Quantity = quantity
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
    }
}
