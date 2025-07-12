using Microsoft.AspNetCore.Mvc;

namespace moon.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View("~/Views/Home/Customer/Index.cshtml");
        public IActionResult Contact() => View("~/Views/Home/Customer/Contact.cshtml");
        public IActionResult Shop() => View("~/Views/Home/Customer/Shop.cshtml");
        public IActionResult About() => View("~/Views/Home/Customer/About.cshtml");
        public IActionResult Cart() => View("~/Views/Home/Customer/Cart.cshtml");
        public IActionResult Checkout() => View("~/Views/Home/Customer/Checkout.cshtml");
        public IActionResult ProductDetail() => View("~/Views/Home/Customer/ProductDetail.cshtml");
        public IActionResult Instruct() => View("~/Views/Home/Customer/Instruct.cshtml");
    }
}
