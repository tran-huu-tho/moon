using Microsoft.AspNetCore.Mvc;

namespace moon.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (email == "admin@gmail.com" && password == "123456")
            {
                TempData["Success"] = "Đăng nhập thành công!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";
                return View();
            }
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string name, string email, string password)
        {
            // Tạm demo, chưa lưu DB
            TempData["Success"] = "Đăng ký thành công!";
            return RedirectToAction("Login");
        }
    }
}
