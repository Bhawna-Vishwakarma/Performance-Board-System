using Microsoft.AspNetCore.Mvc;

namespace Performance_Board_System.Controllers
{
    public class EmployeeController : Controller
    {
        [Route("employee-dashboard")]
        public IActionResult EmployeeDashboard()
        {
            return View();
        }
    }
}
