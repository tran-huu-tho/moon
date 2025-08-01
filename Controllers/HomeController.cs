using Microsoft.AspNetCore.Mvc;
using moon.Controllers;
using moon.Models;
using Microsoft.EntityFrameworkCore;


namespace moon.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index() => View("~/Views/Home/Customer/Index.cshtml");

       public IActionResult Receipt()
        {
            string userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Login"); // hoặc controller bạn dùng để đăng nhập
            }

            var orders = _context.Orders
                .Include(o => o.Items)  // đảm bảo có dữ liệu từ Items
                .Where(o => o.UserId == userId)
                .ToList();

            return View("~/Views/Home/Customer/Receipt.cshtml", orders);
        }


        public IActionResult Contact() => View("~/Views/Home/Customer/Contact.cshtml");
        public IActionResult Shop(int page = 1)
        {
            int pageSize = 12;
            var products = _context.Products.AsQueryable();

            var totalItems = products.Count();
            var pagedProducts = products
                .OrderBy(p => p.Id) // mặc định sắp xếp theo Id
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.SortOrder = ""; // không có sắp xếp khi mặc định

            var categoryCounts = _context.Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    ProductCount = _context.Products.Count(p => p.CategoryId == c.Id)
                })
                .ToList();

            ViewBag.CategoryCounts = categoryCounts;

            return View("~/Views/Home/Customer/Shop.cshtml", pagedProducts);
        }



        public IActionResult About() => View("~/Views/Home/Customer/About.cshtml");
        public IActionResult Cart() => View("~/Views/Home/Customer/Cart.cshtml");
        public IActionResult Checkout() => View("~/Views/Home/Customer/Checkout.cshtml");
        public IActionResult ProductDetail(string id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            return View("Customer/ProductDetail", product);
        }

        public IActionResult Instruct() => View("~/Views/Home/Customer/Instruct.cshtml");
        public IActionResult Profile() => View("~/Views/Home/Customer/Profile.cshtml");

        public IActionResult Customer(string sortOrder, int page = 1)
        {
            int pageSize = 12;
            var products = _context.Products.AsQueryable();

            switch (sortOrder)
            {
                case "price_desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                case "price_asc":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "price_range_0_100":
                    products = products.Where(p => p.Price <= 100000);
                    break;
                case "price_range_100_200":
                    products = products.Where(p => p.Price > 100000 && p.Price <= 200000);
                    break;
                case "price_range_200_9999999":
                    products = products.Where(p => p.Price > 200000);
                    break;
                default:
                    products = products.OrderBy(p => p.Id);
                    break;
            }

            var totalItems = products.Count();
            var pagedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.SortOrder = sortOrder;

            var categoryCounts = _context.Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    ProductCount = _context.Products.Count(p => p.CategoryId == c.Id)
                })
                .ToList();

            ViewBag.CategoryCounts = categoryCounts;

            return View("Customer/Shop", pagedProducts);
        }




        [HttpPost]
        public async Task<IActionResult> SendContact(string name, string email, string subject, string message)
        {
            var emailSender = new EmailSender();

            string to = "ththohttt2211032@student.ctuet.edu.vn";
            string emailSubject = $"[GÓP Ý] {subject ?? "(Không có tiêu đề)"} từ {name}";
            string body = $@"
                <h3>Thông tin góp ý từ khách hàng</h3>
                <p><strong>Họ và tên:</strong> {name}</p>
                <p><strong>Email:</strong> {email}</p>
                <p><strong>Tiêu đề:</strong> {subject}</p>
                <p><strong>Nội dung:</strong></p>
                <p>{message}</p>
                <hr>
                <p style='font-size:12px;color:gray;'>Email này được gửi từ form Liên hệ Moon Shop</p>
            ";

            try
            {
                await emailSender.SendEmailAsync(to, emailSubject, body);
                TempData["Success"] = "Cảm ơn bạn đã gửi ý kiến. Chúng tôi sẽ phản hồi sớm nhất!";
            }
            catch
            {
                TempData["Error"] = "Lỗi khi gửi email. Vui lòng thử lại sau!";
            }

            return RedirectToAction("Contact");
        }

        public IActionResult FilterByCategory(string categoryName)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Name == categoryName);
            if (category == null)
                return RedirectToAction("Shop");

            var filteredProducts = _context.Products
                .Where(p => p.CategoryId == category.Id)
                .ToList();

            var categoryCounts = _context.Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    ProductCount = _context.Products.Count(p => p.CategoryId == c.Id)
                })
                .ToList();

            ViewBag.CategoryCounts = categoryCounts;
            ViewBag.SelectedCategory = categoryName;

            return View("Customer/Shop", filteredProducts);
        }
    }
}
