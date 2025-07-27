using Microsoft.AspNetCore.Mvc;
using moon.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace moon.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CheckoutController(ApplicationDbContext context)
        {
            _context = context;
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

            return View("~/Views/Home/Customer/Checkout.cshtml", cartItems);
        }

        public IActionResult ThankYou()
        {
            return View("~/Views/Home/Customer/ThankYou.cshtml");
        }

        [HttpPost]
        public IActionResult PlaceOrder(
     string FullName,
     string PhoneNumber,
     string Province,
     string District,
     string Ward,
     string Note,
     string PaymentMethod)
        {
            var email = HttpContext.Session.GetString("Email");
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null) return RedirectToAction("Login", "Login");

            var cartItems = _context.CartItems
                .Where(c => c.UserId == user.Id)
                .ToList();

            if (!cartItems.Any()) return RedirectToAction("Index");

            decimal subtotal = cartItems.Sum(item => item.Total);
            decimal shippingFee = 30000;
            decimal total = subtotal + shippingFee;

            // Tạo mã OrderId
            string newOrderId = GenerateNextOrderId();

            // Tạo danh sách OrderItems
            var orderItems = cartItems.Select((c, index) => new OrderItem
            {
                Id = GenerateNextOrderItemId(index),
                ProductId = c.ProductId,
                ProductName = c.ProductName,
                Quantity = c.Quantity,
                UnitPrice = c.Price,
                OrderId = newOrderId
            }).ToList();

            var order = new Order
            {
                Id = newOrderId,
                UserId = user.Id,
                FullName = FullName,
                PhoneNumber = PhoneNumber,
                Province = Province,
                District = District,
                Ward = Ward,
                Note = Note,
                PaymentMethod = PaymentMethod,
                TotalAmount = total,
                ShippingFee = shippingFee,
                OrderDate = DateTime.Now,
                Items = orderItems
            };

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cartItems);
            _context.SaveChanges();

            return RedirectToAction("ThankYou");
        }

        // ======= Tạo mã OrderId: HD001, HD002,... =======
        private string GenerateNextOrderId()
        {
            var lastOrder = _context.Orders
                .Where(o => o.Id != null && o.Id.StartsWith("HD"))
                .OrderByDescending(o => o.Id)
                .FirstOrDefault();

            if (lastOrder != null && lastOrder.Id.Length >= 5)
            {
                string lastNumberStr = lastOrder.Id.Substring(2);
                if (int.TryParse(lastNumberStr, out int lastNumber))
                {
                    return "HD" + (lastNumber + 1).ToString("D3");
                }
            }

            return "HD001";
        }


        // ======= Tạo mã OrderItemId: CTHD001, CTHD002,... =======
        private string GenerateNextOrderItemId(int indexOffset)
        {
            var lastItem = _context.OrderItems
                .Where(i => i.Id != null && i.Id.StartsWith("CTHD"))
                .OrderByDescending(i => i.Id)
                .FirstOrDefault();

            int start = 1;
            if (lastItem != null && lastItem.Id.Length >= 7)
            {
                string lastNumberStr = lastItem.Id.Substring(4);
                if (int.TryParse(lastNumberStr, out int lastNumber))
                {
                    start = lastNumber + 1;
                }
            }

            return "CTHD" + (start + indexOffset).ToString("D3");
        }


    }
}
