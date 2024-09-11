using Microsoft.AspNetCore.Mvc;

namespace SchoolManagement.Controllers
{
    public class StudentController : Controller
    {
        HttpClient client;
        public StudentController()
        {

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, SslPolicyErrors) => { return true; };
            client = new HttpClient(clientHandler);
        }
        public IActionResult Index()
        {
            return View();
        }



    }
}
