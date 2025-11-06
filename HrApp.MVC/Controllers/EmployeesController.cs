using Microsoft.AspNetCore.Mvc;

namespace HrApp.MVC.Controllers
{
    public class EmployeesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
