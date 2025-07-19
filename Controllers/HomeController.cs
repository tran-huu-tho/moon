using Microsoft.AspNetCore.Mvc;
using moon.Controllers;

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
        public IActionResult Profile() => View("~/Views/Home/Customer/Profile.cshtml"); 


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
    }
}
