using Microsoft.AspNetCore.Mvc;

namespace SchoolManagement.Controllers
{
    public class TeacherController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
