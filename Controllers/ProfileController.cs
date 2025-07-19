using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using moon.Models;
using System;
using System.Linq;

namespace moon.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
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

            ViewBag.HoTen = user.Name;
            ViewBag.Email = user.Email;
            ViewBag.Phone = user.Phone ?? "";

            if (user.Avatar != null && user.Avatar.Length > 0)
            {
                string base64 = Convert.ToBase64String(user.Avatar);
                ViewBag.Avatar = $"data:image/png;base64,{base64}";
            }
            else
            {
                ViewBag.Avatar = "/images/avt.jpg";
            }

            return View("~/Views/Home/Customer/Profile.cshtml");
        }
    private void FillUserToViewBag(User user)
    {
        ViewBag.HoTen = user.Name;
        ViewBag.Email = user.Email;
        ViewBag.Phone = user.Phone ?? "";

        if (user.Avatar != null && user.Avatar.Length > 0)
        {
            string base64 = Convert.ToBase64String(user.Avatar);
            ViewBag.Avatar = $"data:image/png;base64,{base64}";
        }
        else
        {
            ViewBag.Avatar = "/images/avt.jpg";
        }
    }

       [HttpPost]
    public IActionResult Index(string name, string phone, string oldPassword, string newPassword, string confirmPassword, IFormFile Avatar)
    {
        var email = HttpContext.Session.GetString("Email");

        if (string.IsNullOrEmpty(email))
            return RedirectToAction("Login", "Login");

        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        if (user == null)
            return RedirectToAction("Login", "Login");

        bool isChanged = false;

        if (user.Name != name)
        {
            user.Name = name;
            isChanged = true;
        }

        if (user.Phone != phone)
        {
            user.Phone = phone;
            isChanged = true;
        }

        if (!string.IsNullOrEmpty(oldPassword))
        {
            if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ mật khẩu.";
                FillUserToViewBag(user);
                return View("~/Views/Home/Customer/Profile.cshtml");
            }

            if (user.Password != oldPassword)
            {
                ViewBag.Error = "Mật khẩu cũ không đúng.";
                FillUserToViewBag(user);
                return View("~/Views/Home/Customer/Profile.cshtml");
            }

            if (newPassword == oldPassword)
            {
                ViewBag.Error = "Mật khẩu mới không được trùng với mật khẩu cũ.";
                FillUserToViewBag(user);
                return View("~/Views/Home/Customer/Profile.cshtml");
            }

            if (newPassword.Length < 8 || newPassword.Length > 16 || !newPassword.Any(char.IsLetter) || !newPassword.Any(char.IsDigit))
            {
                ViewBag.Error = "Mật khẩu mới phải dài 8–16 ký tự, bao gồm cả chữ và số.";
                FillUserToViewBag(user);
                return View("~/Views/Home/Customer/Profile.cshtml");
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu mới và xác nhận mật khẩu không khớp.";
                FillUserToViewBag(user);
                return View("~/Views/Home/Customer/Profile.cshtml");
            }

            user.Password = newPassword;
            isChanged = true;
        }

        if (Avatar != null && Avatar.Length > 0)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                Avatar.CopyTo(ms);
                user.Avatar = ms.ToArray();
                isChanged = true;
            }
        }

        if (!isChanged)
        {
            return RedirectToAction("Index");
        }

        _context.SaveChanges();
        TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
        return RedirectToAction("Index");
    }

    }
}
