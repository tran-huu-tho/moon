using Microsoft.AspNetCore.Mvc;

namespace moon.Controllers
{
    public class ManagerController : Controller
    {
        public IActionResult Index() => View("~/Views/Home/Manager/Index.cshtml"); 
    }
}
