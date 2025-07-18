using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace moon.Controllers
{
    public class EmailSender
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var fromAddress = new MailAddress("dungnguyen6904@gmail.com", "Moon");
            var toAddress = new MailAddress(toEmail);
            const string fromPassword = "lunz iygo mudu ghhq"; 

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            await smtp.SendMailAsync(message);
        }
    }
}
