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
                    return RedirectToAction("Student", "Index");
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
                    
                    return RedirectToAction("Teacher", "Index");
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
            string url = "https://localhost:44379/api/Admin/AddStudent";
            var jsondata = JsonConvert.SerializeObject(a);
            StringContent content = new StringContent(jsondata, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                
               
                //studentEmail();
                return RedirectToAction("Addstudent");
            }
            return View();
        }
        public ActionResult studentEmail()
        {
            try
            {

                string toEmail = "prasadmhasal@gmail.com";
                string subject = "Test Email from ASP.NET MVC";
                string body = $"Hii student you are addmited in our school your portal UserName :   And Password ";


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
        public IActionResult LeaveStatus(int id , string status)
        {
            if (status == "Approve")
            {
                string url = "";
            }

        return View(); 
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

        
       
    }
}
