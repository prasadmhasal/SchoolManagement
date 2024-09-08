using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolManagement.Model;
using System.Net.Mail;
using System.Net;

namespace SchoolManagement.Controllers
{
    public class AdminController : Controller
    {
        HttpClient client;
        public AdminController()
        {

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, SslPolicyErrors) => { return true; };
            client = new HttpClient(clientHandler);
        }
        public IActionResult Index()
        {
            return View();
           
        }

        public IActionResult StudentApplyRequest()
        {
            List<StudentRequest> student = new List<StudentRequest>();
            string url = "https://localhost:44379/api/Admin/GetStudentRequest";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                var jsondata = response.Content.ReadAsStringAsync().Result;
                student = JsonConvert.DeserializeObject<List<StudentRequest>>(jsondata);

            }
            return View(student);

        }

        [HttpPost]
        public IActionResult StudentApprove(int id)
        {

            string url = $"https://localhost:44379/api/Admin/StudentRequestDelete/{id}";
            HttpResponseMessage response = client.DeleteAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                SendEmail();
                return RedirectToAction("StudentApplyRequest");
            }
            return View("StudentApplyRequest");
        }

        public ActionResult SendEmail()
        {
            try
            {
                
                string toEmail = "prasadmhasal@gmail.com"; 
                string subject = "Test Email from ASP.NET MVC";
                string body = "This is a test email sent from an ASP.NET MVC application.";

               
                var fromAddress = new MailAddress("prasadmhasal@gmail.com.com", "Prasad");
                var toAddress = new MailAddress(toEmail);
                string fromPassword = "fxjuqdrhzmmeksyq"; 

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com", 
                    Port = 587, 
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

          
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                   
                    smtp.Send(message);
                }

                ViewBag.EmailStatus = "Email sent successfully!";
            }
            catch (Exception ex)
            {
                ViewBag.EmailStatus = "Error while sending email: " + ex.Message;
            }

            return View();
        }

    }
}
