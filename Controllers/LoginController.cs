using Microsoft.AspNetCore.Mvc;
using moon.Models;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using moon.Controllers; 

namespace moon.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ======= LOGIN =======
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

         [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                TempData["Success"] = "Đăng nhập thành công!";
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("Phone", user.Phone ?? "");

                // ✅ Thêm dòng này để lưu UserId
                 HttpContext.Session.SetString("UserId", user.Id);
 
                    string avatarString;
                if (user.Avatar != null)
                {

                    string base64Avatar = Convert.ToBase64String(user.Avatar);
                    avatarString = $"data:image/png;base64,{base64Avatar}";
                }
                else
                {
                    avatarString = "/images/avt.jpg";
                }

                HttpContext.Session.SetString("Avatar", avatarString);

                if (user.Role)
                    return RedirectToAction("Index", "Manager");
                else
                    return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";
            return View();
        }

        // ======= REGISTER =======
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string name, string email, string password)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == email);
            if (existingUser != null)
            {
                ViewBag.Error = "Email đã được sử dụng, vui lòng chọn email khác!";
                return View();
            }

            var avatarPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/avt.jpg");
            byte[] avatarBytes = System.IO.File.Exists(avatarPath) ? System.IO.File.ReadAllBytes(avatarPath) : Array.Empty<byte>();

            var newUser = new User
            {
                Id = GenerateNextUserId(),
                Name = name,
                Email = email,
                Password = password,
                Phone = "",
                Role = false,
                Avatar = avatarBytes
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            // === GỬI EMAIL CHÀO MỪNG ===
            var emailSender = new EmailSender();
            string subject = "Chào mừng bạn đến với Moon Shop!";
            string body = $@"
                <h2>🌙 Chào mừng {name} đến với Moon Shop!</h2>
                <p>Cảm ơn bạn đã đăng ký tài khoản tại <strong>Moon</strong> – nơi chuyên cung cấp các phụ kiện anime chất lượng.</p>
                <p>Bạn đã sẵn sàng khám phá thế giới anime chưa?</p>
                <p>Hãy <a href='http://localhost:9999/'>truy cập cửa hàng</a> để xem ngay các sản phẩm mới nhất!</p>
                <hr>
                <p style='font-size:12px;color:gray;'>Đây là email tự động, vui lòng không trả lời.</p>
            ";
            await emailSender.SendEmailAsync(email, subject, body);

            TempData["Success"] = "Đăng ký thành công! Hãy đăng nhập.";
            return RedirectToAction("Login");
            
        }

        // ======= LOGOUT =======
        public IActionResult Logout()
        {
            // Xóa session
            HttpContext.Session.Clear();

            // Xóa luôn thông báo TempData
            TempData.Clear();

            return RedirectToAction("Index", "Home");
        }

        // ======= AUTO ID (ND01, ND02...) =======
        private string GenerateNextUserId()
        {
            var lastUser = _context.Users
                .Where(u => u.Id.StartsWith("ND"))
                .OrderByDescending(u => u.Id)
                .FirstOrDefault();

            if (lastUser != null && lastUser.Id.Length >= 4)
            {
                string lastNumberStr = lastUser.Id.Substring(2);
                if (int.TryParse(lastNumberStr, out int lastNumber))
                {
                    int nextNumber = lastNumber + 1;
                    return "ND" + nextNumber.ToString("D2");
                }
            }

            return "ND01";
        }
        
    }
}
