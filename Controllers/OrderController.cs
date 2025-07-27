using Microsoft.AspNetCore.Mvc;
using moon.Models;
using System.Linq;

namespace moon.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult StatusOrder()
        {
            var orders = _context.Orders.ToList();
            return View(orders);
        }
    }
}
