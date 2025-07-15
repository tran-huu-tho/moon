using Microsoft.AspNetCore.Mvc;

namespace moon.Controllers
{
    public class ManagerController : Controller
    {
        public IActionResult Index() => View("~/Views/Home/Manager/Index.cshtml");
        public IActionResult Product() => View("~/Views/Home/Manager/Product.cshtml");
        public IActionResult Category() => View("/Views/Home/Manager/Category.cshtml");
        public IActionResult Invoice() => View("/Views/Home/Manager/Invoice.cshtml");
        public IActionResult Statistical() => View("/Views/Home/Manager/Statistical.cshtml");
        public IActionResult StatusOrder() => View("/Views/Home/Manager/StatusOrder.cshtml");
    }
}
