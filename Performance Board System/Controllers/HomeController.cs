using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Performance_Board_System.Models;

namespace Performance_Board_System.Controllers
{
    [Authorize] //revent direct access of methos only can after login 
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)] //after loggedout prevent redirect with cache data
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Getting Employee Dashboard.
        /// </summary>
        /// <returns></returns>
        [Route("dashboard")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get Employee Rating.
        /// </summary>
        /// <returns></returns>
        [Route("my-rating")]
        public IActionResult Rating()
        {
            return View();
        }


        [Route("privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
