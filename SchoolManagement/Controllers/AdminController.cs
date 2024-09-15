using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolManagement.Model;
using System.Net.Mail;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using System.Xml;
using Azure;
using System.Diagnostics;

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

		public IActionResult SignIn()
		{
			return View();
		}

        [HttpPost]
        public IActionResult SignIn (Users u )   
        {
            u.Urole = "empty";
            string url = $"https://localhost:44379/api/Admin/SignIn";
            var jsondata = JsonConvert.SerializeObject(u); 
            StringContent content = new StringContent(jsondata, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
               
                var responseData = response.Content.ReadAsStringAsync().Result;
                var userData = JsonConvert.DeserializeObject<Users>(responseData); // Replace `Users` with your expected return type

                
                if (userData.Urole == "Student")
                {
                    var identity = new ClaimsIdentity(new[] {
                   new Claim(ClaimTypes.Name,userData.UserName)},
                   CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    HttpContext.Session.SetString("UserName", userData.UserName);
                    HttpContext.Session.SetString("Urole", userData.Urole);
                    return RedirectToAction("Index", "Student");
                }
                else if (userData.Urole == "Teacher")
                {
                    var identity = new ClaimsIdentity(new[] {
                   new Claim(ClaimTypes.Name,userData.UserName)},
                    CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    HttpContext.Session.SetString("UserName", userData.UserName);
                    HttpContext.Session.SetString("Urole", userData.Urole);
                    
                    return RedirectToAction("Index", "Teacher");
                }
                else if (userData.Urole == "Admin")
                {
                    var identity = new ClaimsIdentity(new[] {
                   new Claim(ClaimTypes.Name,userData.UserName)},
                    CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    HttpContext.Session.SetString("UserName", userData.UserName);
                    HttpContext.Session.SetString("Urole", userData.Urole);
                    return RedirectToAction("Index");
                }
                else if (userData.Urole == "labrarian")
                {
                    var identity = new ClaimsIdentity(new[] {
                   new Claim(ClaimTypes.Name,userData.UserName)},
                    CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    HttpContext.Session.SetString("UserName", userData.UserName);
                    HttpContext.Session.SetString("Urole", userData.Urole);
                    return RedirectToAction("Index","Librarian");
                }
                else
                {
                   
                    TempData["SignInError"] = "Unknown role!";
                    return RedirectToAction("SignIn");
                }
            }
            else
            {
                
                TempData["SignInError"] = "Invalid username or password";
                return RedirectToAction("SignIn");
            }
           
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

            string url = $"https://localhost:44379/api/Admin/StudentRequestDelete/";
            HttpResponseMessage response = client.DeleteAsync(url+id).Result;
            if (response.IsSuccessStatusCode)
            {
                SendEmail();
                TempData["StudentApprove"] = "Student Approve And Email Sended ";
              
            }
            else
            {
            }
           return RedirectToAction("StudentApplyRequest");
            
        }

        public ActionResult SendEmail()
        {
            try
            {
                
                string toEmail = "prasadmhasal@gmail.com"; 
                string subject = "Test Email from ASP.NET MVC";
                string body = "This is a test email sent from an ASP.NET MVC application.";

               
                var fromAddress = new MailAddress("prasadmhasal@gmail.com", "Prasad");
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

        public IActionResult AddStudent()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddStudent(AddStudent a)
        {
            a.AddMisstiondate = "empty";
            a.FeesStatus = "Pending";
            string url = "https://localhost:44379/api/Admin/AddStudent";
            var jsondata = JsonConvert.SerializeObject(a);
            StringContent content = new StringContent(jsondata, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                // Send email to the student with their User and Password
                SendStudentCredentials(a.Email, a.StudentUser, a.Studentpass);
                TempData["StudentEmailSend"] = "Email Sended To Student";

                return RedirectToAction("Addstudent");
            }

            return View();
        }

        public void SendStudentCredentials(string toEmail, string studentUser, string studentPass)
        {
            try
            {
                var fromEmail = new MailAddress("prasadmhasal@gmail.com", "Your Name or Organization");
                var to = new MailAddress(toEmail);
                var fromPassword = "fxjuqdrhzmmeksyq";  // Make sure you secure this in config
                string subject = "Your Student Login Credentials";
                string body = $"Dear Student, \n\nYour login credentials are as follows:\nUsername: {studentUser}\nPassword: {studentPass}\n\nPlease keep them safe.";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com", // Example: smtp.gmail.com
                    Port = 587, // Or 465, depending on your email provider's requirement
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromEmail.Address, fromPassword)
                };

                using (var message = new MailMessage(fromEmail, to)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                // Handle any error in sending email (log it)
            }
        }

        

        public IActionResult AddTeacher()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddTeacher(AddTeacher t)
        {
            t.Joindate = "Empty";
            string url = "https://localhost:44379/api/Admin/AddTeacher";
            var jsondata = JsonConvert.SerializeObject(t);
            StringContent content = new StringContent(jsondata, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(url,content).Result;
            if (response.IsSuccessStatusCode) 
            {
				SendStudentCredentials(t.TeacherEmail, t.TeacherUser, t.Teacherpass);
				TempData["AddTeacher"] = "Successfully Added Teacher ";
                return RedirectToAction("AddTeacher");
            }
            return View();
        }

        [HttpGet]
        public IActionResult LeaveRequest(TeacherLeaveRequest t)
        {
            List<TeacherLeaveRequest> Teacher = new List<TeacherLeaveRequest>();
            string url = "https://localhost:44379/api/Admin/GetLeaveRequest";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                var jsondata = response.Content.ReadAsStringAsync().Result;
                Teacher = JsonConvert.DeserializeObject<List<TeacherLeaveRequest>>(jsondata);

            }
            return View(Teacher);
            
        }

		[HttpPost]
		public IActionResult LeaveStatus(int Id, string status)
		{
			string url = $"https://localhost:44379/api/Admin/PostLeaveRequest/";
			HttpResponseMessage response = client.DeleteAsync(url + Id).Result;

			if (response.IsSuccessStatusCode)
			{
				// Check the status and send the corresponding email
				if (status == "Approve")
				{
					TempData["EmailSend"] = "Email Sended";
					SendEmail("Approve", "prasadmhasal@gmail.com");
				}
				else if (status == "Rejected")
				{
					TempData["EmailSend"] = "Email Sended";
					SendEmail("Rejected", "prasadmhasal@gmail.com");
				}

				
			}

			return RedirectToAction("LeaveRequest");
		}


		public void SendEmail(string status, string toEmail)
		{
			try
			{
				string subject = "Leave Request " + status;
				string body = "";

				// Customize the email body based on approval/rejection
				if (status == "Approve")
				{
					body = "Dear Student,\n\nYour leave request has been approved. We hope you return refreshed and ready to resume your work!\n\nBest Regards,\nSchool Administration";
				}
				else if (status == "Rejected")
				{
					body = "Dear Student,\n\nWe regret to inform you that your leave request has been rejected. For more details, please contact the administration.\n\nBest Regards,\nSchool Administration";
				}

				var fromAddress = new MailAddress("prasadmhasal@gmail.com", "Prasad");
				var toAddress = new MailAddress(toEmail);
				string fromPassword = "fxjuqdrhzmmeksyq"; // Be sure to secure this in production

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
		}



		public IActionResult AddLabrarian() 
        { 
            return View();
        }

        [HttpPost]
        public IActionResult AddLabrarian(AddLibrarian l)
        {
            l.Joindate = "Empty";
            string url = $"https://localhost:44379/api/Admin/Addlabrarian";
            var jsondata = JsonConvert.SerializeObject(l);
            StringContent content = new StringContent(jsondata, Encoding.UTF8, "application/Json");
            HttpResponseMessage response = client.PostAsync(url , content).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Addlabrarian"] = "Successfully Added Labrarian ";
                return RedirectToAction("AddLabrarian");
            }
            return View();
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

        public IActionResult AddEvent()
        {
            var newEvent = new Event();
            return View(newEvent);
        }

        [HttpPost]
        public IActionResult AddEvent(Event newEvent)
        {
            if (newEvent == null)
            {
                return BadRequest("Event data is null.");
            }

            string url = "https://localhost:44379/api/Admin/CreateEvent";
            var jsonData = JsonConvert.SerializeObject(newEvent);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage result = client.PostAsync(url, stringContent).Result;

            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction("Calendar");
            }

            return View(newEvent);
        }

        public IActionResult UpdateEvent(int id)
        {
            string url = $"https://localhost:44379/api/Admin/updateEvent/{id}";

            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                var jsonData = response.Content.ReadAsStringAsync().Result;
                var eventObj = JsonConvert.DeserializeObject<Event>(jsonData);

                if (eventObj != null)
                {
                    return View(eventObj);
                }
            }

            return NotFound();
        }
        [HttpPost]
        public IActionResult UpdateEvent(Event updatedEvent)
        {
            string url = "https://localhost:44379/api/Event/UpdateEvent";
            var jsonData = JsonConvert.SerializeObject(updatedEvent);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage result = client.PutAsync(url, stringContent).Result;

            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction("Calendar");
            }

            return View(updatedEvent);
        }

        public IActionResult DeleteEvent(int id)
        {
            string url = $"https://localhost:44379/api/Admin/DeleteEvent/{id}";

            HttpResponseMessage result = client.DeleteAsync(url).Result;

            if (result.IsSuccessStatusCode)
            {
                return Ok();
            }

            return BadRequest();
        }

        public IActionResult AddTimetable()
        {
            return View();

        }
        [HttpPost]
        public IActionResult AddTimetable(Timetable tt)
        {
          
            string url = "https://localhost:44379/api/Admin/AddTimeTable";
            var jsondata = JsonConvert.SerializeObject(tt);
            StringContent content = new StringContent(jsondata, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Msg"] = "TimeTable Added Successfully";
                return RedirectToAction("AddTimetable");
            }
            else
            {
                TempData["Msg"] = "Couldn't add Timetable please try again";
                return View();
            }
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
        public JsonResult FetchSubjects()
        {
            List<Subject> subjects = new List<Subject>();
            string url = "https://localhost:44379/api/Admin/FetchSubject";
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                var jsondata = response.Content.ReadAsStringAsync().Result;
                subjects = JsonConvert.DeserializeObject<List<Subject>>(jsondata);
            }

            return Json(subjects);
        }


        [HttpGet]
        public JsonResult FetchClass()
        {
            List<AddTeacher> Class = new List<AddTeacher>();
            string url = "https://localhost:44379/api/Admin/FetchClass";
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                var jsondata = response.Content.ReadAsStringAsync().Result;
                Class = JsonConvert.DeserializeObject<List<AddTeacher>>(jsondata);
            }

            return Json(Class);
        }


    }
}
