using Microsoft.AspNetCore.Mvc;

namespace moon.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Contact() => View();
        public IActionResult Shop() => View();
        public IActionResult About() => View();
        public IActionResult Cart() => View();
        public IActionResult Checkout() => View();
        public IActionResult ProductDetail() => View();
        public IActionResult Instruct() => View();
    }
}
