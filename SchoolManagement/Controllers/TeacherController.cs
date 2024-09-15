using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolManagement.Model;
using System.Text;

namespace SchoolManagement.Controllers
{
    public class TeacherController : Controller
    {
        HttpClient client;
        public TeacherController()
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
        public IActionResult TeacherProfile(AddTeacher e)
        {
            
            AddTeacher empList = new AddTeacher();
            var username = HttpContext.Session.GetString("UserName");
            string url = $"https://localhost:44379/api/Teacher/TeacherProfile/{username}";
            HttpResponseMessage response = client.GetAsync(url).Result;
            
            if (response.IsSuccessStatusCode)
            {
                var jsondata = response.Content.ReadAsStringAsync().Result;
                var obj = JsonConvert.DeserializeObject<AddTeacher>(jsondata);
                if (obj != null)
                {
                    empList = obj;
                    HttpContext.Session.SetString("Standard", empList.Standard);
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

        public ActionResult RequestLeave()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RequestLeave(TeacherLeaveRequest e)
        {
            var UserName  = HttpContext.Session.GetString("UserName");
            var Standard = HttpContext.Session.GetString("Standard");
            e.UserName = UserName;
            e.Standard = Standard;
            string url = "https://localhost:44379/api/Teacher/RequestLeave";
            var jsondata = JsonConvert.SerializeObject(e);
            StringContent content = new StringContent(jsondata, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["LeaveEmail"] = "Request Send Successfully";
                return RedirectToAction("RequestLeave");
            }
            return View();
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


        [HttpGet]
        public IActionResult AddAttendance()
        {
            List<AddStudent> emps = new List<AddStudent>();
           var Standard = HttpContext.Session.GetString("Standard");
            string url = $"https://localhost:44379/api/Teacher/GetAttendance/{Standard}";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                var jsondata = response.Content.ReadAsStringAsync().Result;
                var obj = JsonConvert.DeserializeObject<List<AddStudent>>(jsondata);
                if (obj != null)
                {
                    emps = obj;
                    return View(emps); //Added this line
                }
            }

            return View(emps);
        }

        [HttpPost]
        public IActionResult AddAttendance(string[] Attendance)
        {
            List<Attendance> attendanceList = new List<Attendance>();

            using (HttpClient client = new HttpClient())
            {
                foreach (var studentId in Attendance)
                {
                  
                    var getUrl = $"https://localhost:44379/api/Teacher/GetStudentById/{studentId}";

                    
                    HttpResponseMessage getResponse = client.GetAsync(getUrl).Result;

                    if (getResponse.IsSuccessStatusCode)
                    {
                        
                        var jsonData = getResponse.Content.ReadAsStringAsync().Result;
                        var student = JsonConvert.DeserializeObject<AddStudent>(jsonData);

                        if (student != null)
                        {
                            
                            attendanceList.Add(new Attendance
                            {
                                StudentId = student.StudentId,
                                FullName = student.fullname,
                                Standard = student.Standard,
                                Date = DateTime.Now.ToString("dd-MM-yyyy"),
                                IsPresent = true 
                            });
                        }
                        TempData["Attendance"] = $"All Attendance Marked.";
                    }
                    else
                    {
                        
                        TempData["Message"] = $"Error fetching student with ID {studentId}.";
                        return View();
                    }
                  }

          
                string postUrl = "https://localhost:44379/api/Teacher/AssignAttendance";
                var jsondata = JsonConvert.SerializeObject(attendanceList);
                StringContent content = new StringContent(jsondata, Encoding.UTF8, "application/json");

                HttpResponseMessage postResponse = client.PostAsync(postUrl, content).Result;

                if (postResponse.IsSuccessStatusCode)
                {
                    TempData["Message"] = "Attendance submitted successfully!";
                    return RedirectToAction("AddAttendance");
                }
                else
                {
                    TempData["Message"] = "Error submitting attendance.";
                    return View();
                }
            }
        }





    }
}
