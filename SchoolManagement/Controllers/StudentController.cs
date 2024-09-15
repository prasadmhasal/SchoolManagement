using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SchoolManagement.Model;
using System.Text;

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

        [HttpGet]
        public IActionResult Profile(AddStudent e)
        {

            AddStudent empList = new AddStudent();
            var username = HttpContext.Session.GetString("UserName");
            string url = $"https://localhost:44379/api/Student/Profile/{username}";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                var jsondata = response.Content.ReadAsStringAsync().Result;
                var obj = JsonConvert.DeserializeObject<AddStudent>(jsondata);
                if (obj != null)
                {
                    empList = obj;
                }
            }
            return View(empList);
        }

        public IActionResult Calendar()
        {
            return View();
        }

        public IActionResult GetEvents()
        {
            List<Event> eventList = new List<Event>();
            string url = "https://localhost:44379/api/Admin/GetEvents";

            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                var jsonData = response.Content.ReadAsStringAsync().Result;
                eventList = JsonConvert.DeserializeObject<List<Event>>(jsonData);
            }

            var calendarEvents = eventList.Select(e => new
            {
                id = e.Id,
                title = e.Title,
                start = e.StartDate.ToString("yyyy-MM-dd"),
                end = e.EndDate.ToString("yyyy-MM-dd"),
                description = e.Description,
                isAcademic = e.IsAcademic
            });

            return Json(calendarEvents);
        }
        public IActionResult GetTimeTable()
        {
            List<Timetable> teach = new List<Timetable>();

            string url = "https://localhost:44379/api/Admin/GetTimeTable";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                var jsondata = response.Content.ReadAsStringAsync().Result;
                teach = JsonConvert.DeserializeObject<List<Timetable>>(jsondata);
            }
            return View(teach);
        }

        public IActionResult FeesPay()
        {

            List<AddStudent> Fees = new List<AddStudent>();
            var StudentUser = HttpContext.Session.GetString("UserName");
            string url = $"https://localhost:44379/api/Admin/FeesPay/{StudentUser}";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                var jsondata = response.Content.ReadAsStringAsync().Result;
                Fees = JsonConvert.DeserializeObject<List<AddStudent>>(jsondata);
            }
            return View(Fees);

        }


        [HttpPost]
        public IActionResult InitiatePayment(string StudentUser, string Email, double Fees)
        {
        
            var client = new Razorpay.Api.RazorpayClient("rzp_test_hyxzlMmdpXpNKr", "GWmkim1me8JM0XvpIucQwGCx");

        
            var paymentAmount = Fees * 100;

            var options = new Dictionary<string, object>
        {
            { "amount", paymentAmount },
            { "currency", "INR" },
            { "receipt", Guid.NewGuid().ToString() },
            { "payment_capture", 1 }
        };
            var order = client.Order.Create(options);

           
            return Json(new
            {
                key = "rzp_test_hyxzlMmdpXpNKr", 
                amount = paymentAmount,
                studentName = StudentUser,
                email = Email,
                orderId = order["id"].ToString() 
            });
        }

        [HttpPut]
        public IActionResult UpdateFeeStatus(string StudentUser)
        {
      
            string url = $"https://localhost:44379/api/Student/UpdateFeesStatus/{StudentUser}";

          
            var jsonData = JsonConvert.SerializeObject(new { FeesStatus = "Paid" });  
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

       
            HttpResponseMessage response = client.PutAsync(url, content).Result;  

            if (response.IsSuccessStatusCode)
            {
               
                // SendStudentCredentials(a.Email, a.StudentUser, a.Studentpass);
                TempData["StudentEmailSend"] = "Email sent to student.";
                return RedirectToAction("FeesPay");
            }

          
            ViewBag.ErrorMessage = "Failed to update the fee status. Please try again.";
            return View();
        }





    }
}
