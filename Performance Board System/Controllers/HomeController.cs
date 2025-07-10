using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Performance_Board_System.Models;
using Performance_Board_System.Repository.Interfaces;

namespace Performance_Board_System.Controllers
{
    [Authorize] //revent direct access of methos only can after login 
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)] //after loggedout prevent redirect with cache data
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository _userRepository;

        public HomeController(ILogger<HomeController> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
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
        /// Get Employee Feedback.
        /// </summary>
        /// <returns></returns>
        [Route("feedbacks")]
        public IActionResult Feedback()
        {
            return View();
        }

        /// <summary>
        /// Get Employee Attendance.
        /// </summary>
        /// <returns></returns>
        //[Route("attendance")]
        //public async Task<IActionResult> Attendance()
        //{
        //    int userId = GetLoggedInUserId();

        //    DateTime today = DateTime.Today;
        //    DateTime monday = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);

        //    var weekDates = Enumerable.Range(0, 5)
        //        .Select(offset => monday.AddDays(offset))
        //        .ToList();

        //    var allAttendance = await _userRepository.GetWeeklyAttendance(userId, monday, monday.AddDays(4)).ConfigureAwait(false);

        //    var weeklyAttendance = weekDates.Select(date =>
        //    {
        //        var record = allAttendance.FirstOrDefault(a => a.Date.Date == date.Date);
        //        return new AttendanceViewModel
        //        {
        //            Date = date,
        //            FullName = record?.FullName ?? "-",
        //            CheckInTime = string.IsNullOrWhiteSpace(record?.CheckInTime) ? "-" : record.CheckInTime,
        //            CheckOutTime = string.IsNullOrWhiteSpace(record?.CheckOutTime) ? "-" : record.CheckOutTime,
        //            Status = string.IsNullOrWhiteSpace(record?.Status) ? "-" : record.Status,
        //        };
        //    }).ToList();

        //    return View(weeklyAttendance);
        //}


        /// <summary>
        /// Get Employee Attendance by Date.
        /// </summary>
        /// <returns></returns>
        //[HttpPost]
        //[Route("attendance-filter")]
        //public async Task<IActionResult> AttendanceFilter(DateTime? dateFilter)
        //{
        //    var userId = GetLoggedInUserId();

        //    if (dateFilter.HasValue)
        //    {
        //        var filteredData = await _userRepository.GetAttendanceByDateAsync(dateFilter.Value);
        //        ViewBag.IsFilterApplied = true;
        //        return View("Attendance", filteredData); // Ensure this points to the correct view
        //    }
        //    else
        //    {
        //        var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1); // Monday
        //        var endOfWeek = startOfWeek.AddDays(4);
        //        var weeklyData = await _userRepository.GetWeeklyAttendance(userId, startOfWeek, endOfWeek);
        //        ViewBag.IsFilterApplied = false;
        //        return View("Attendance", weeklyData); // Reuse the same view
        //    }
        //}



        /// <summary>
        /// post Employee Attendance.
        /// </summary>
        /// <returns></returns>
        //[HttpPost]
        //[Route("mark-attendance")]
        //public IActionResult MarkAttendance(string ActionType, string Status)
        //{
        //    int userId = GetLoggedInUserId(); // Replace with your session logic
        //    DateTime today = DateTime.Today;
        //    TimeSpan? checkIn = null;
        //    TimeSpan? checkOut = null;

        //    if (ActionType == "CheckIn")
        //        checkIn = DateTime.Now.TimeOfDay;

        //    if (ActionType == "CheckOut")
        //        checkOut = DateTime.Now.TimeOfDay;

        //    var result = _userRepository.MarkAttendance(userId, today, checkIn, checkOut, Status);

        //    TempData["Message"] = result switch
        //    {
        //        1 => "Check-In marked successfully.",
        //        2 => "Check-Out marked successfully.",
        //        -1 => "Cannot check-out before check-in.",
        //        -2 => "You’ve already checked out today.",
        //        -3 => "You’ve already checked in today.",
        //        _ => "Something went wrong."
        //    };

        //    TempData["MessageType"] = result switch
        //    {
        //        1 => "success",
        //        2 => "success",
        //        -1 => "info",
        //        -2 => "warning",
        //        -3 => "warning",
        //        _ => "danger"
        //    };

        //    return RedirectToAction("Attendance");
        //}

        //private int GetLoggedInUserId()
        //{
        //    // Convert the string value from the session to an integer
        //    string userIdString = HttpContext.Session.GetString("UserId");
        //    if (int.TryParse(userIdString, out int userId))
        //    {
        //        return userId;
        //    }
        //    else
        //    {
        //        throw new InvalidOperationException("Invalid UserId in session.");
        //    }
        //}

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
