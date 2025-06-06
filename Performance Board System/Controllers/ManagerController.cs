using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace Performance_Board_System.Controllers
{
    public class ManagerController : Controller
    {
        public IActionResult ManagerDashboard()
        {
            return View();
        }
    }
}
