using Microsoft.AspNetCore.Mvc;

namespace SchoolManagement.Controllers
{
    public class LibrarianController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
