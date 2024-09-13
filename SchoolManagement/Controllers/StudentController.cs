using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolManagement.Model;

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


    }
}
