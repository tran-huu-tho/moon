using Microsoft.AspNetCore.Mvc;
using moon.Models;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

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
                HttpContext.Session.SetString("UserName", user.Name); // Ghi tên vào session

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
        public IActionResult Register(string name, string email, string password)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == email);
            if (existingUser != null)
            {
                ViewBag.Error = "Email đã được sử dụng, vui lòng chọn email khác!";
                return View();
            }

            var newUser = new User
            {
                Id = GenerateNextUserId(),
                Name = name,
                Email = email,
                Password = password,
                Phone = "",
                Role = false,
                Avatar = null
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

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
