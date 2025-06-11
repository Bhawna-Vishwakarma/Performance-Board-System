using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Performance_Board_System.Models;
using Performance_Board_System.Repository.Interfaces;

namespace Performance_Board_System.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IAttendanceRepository _attendanceRepo;

        public EmployeeController(IAttendanceRepository attendanceRepository)
        {
            _attendanceRepo = attendanceRepository;
        }

        [HttpGet("employee-dahboard")]
        public IActionResult EmployeeDashboard()
        {
            return View();
        }

        //[HttpGet("mark-attendance")]
        //public async Task<IActionResult> MarkAttendance()
        //{
        //    ViewBag.StatusList = await _attendanceRepo.GetAllStatusesAsync();
        //    return View();
        //}

        private int GetLoggedInUserId()
        {
            // Convert the string value from the session to an integer
            string userIdString = HttpContext.Session.GetString("UserId");
            if (int.TryParse(userIdString, out int userId))
            {
                return userId;
            }
            else
            {
                throw new InvalidOperationException("Invalid UserId in session.");
            }
        }

        [HttpGet("attendance")]
        public async Task<IActionResult> Attendance()
        {
            //int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            int userId = GetLoggedInUserId();
            //var records = await _attendanceRepo.GetWeeklyAttendanceAsync(userId);
            var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1); // Monday
            var weekDates = Enumerable.Range(0, 5).Select(i => startOfWeek.AddDays(i)).ToList(); // Mon-Fri

            var records = await _attendanceRepo.GetAttendanceForUserInDateRange(userId, startOfWeek, startOfWeek.AddDays(4));

            var viewModel = weekDates.Select(date =>
            {
                var record = records.FirstOrDefault(r => r.Date.Date == date.Date);
                return new WeeklyAttendanceViewModel
                {
                    Date = date,
                    CheckInTime = record?.CheckInTime,
                    CheckOutTime = record?.CheckOutTime,
                    StatusName = record?.StatusName,
                    Remarks = record?.Remarks,
                    IsToday = date.Date == DateTime.Today,
                    IsCheckedIn = record != null && record.CheckInTime != null
                };
            }).ToList();

            return View(viewModel);
            //return View(records);
        }

        [HttpGet("attendance/checkin")]
        public async Task<IActionResult> CheckIn()
        {
            var statuses = await _attendanceRepo.GetAllStatusesAsync();
            ViewBag.Statuses = statuses.Select(s => new SelectListItem
            {
                Value = s.StatusId.ToString(),
                Text = s.StatusName
            });
            return View(new AttendanceCheckInModel());
        }

        [HttpPost("attendance/checkin")]
        public async Task<IActionResult> CheckIn(AttendanceCheckInModel model)
        {
            if (!ModelState.IsValid)
            {
                var statuses = await _attendanceRepo.GetAllStatusesAsync();
                ViewBag.Statuses = statuses.Select(s => new SelectListItem
                {
                    Value = s.StatusId.ToString(),
                    Text = s.StatusName
                });
                return View(model);
            }
            model.UserId = GetLoggedInUserId();
            int result = await _attendanceRepo.CheckInAsync(model);
            TempData["Message"] = result switch
            {
                1 => "Checked in successfully.",
                -1 => "Already checked in today.",
                _ => "Something went wrong."
            };
            return RedirectToAction("Attendance");
        }

        [HttpPost("attendance/checkout")]
        public async Task<IActionResult> CheckOut()
        {
            int userId = GetLoggedInUserId();

            int result = await _attendanceRepo.CheckOutAsync(userId);
            TempData["Message"] = result switch
            {
                1 => "Checked out successfully.",
                -1 => "Check-in not found or already checked out.",
                _ => "Something went wrong."
            };
            return RedirectToAction("Attendance");
        }



    }
}
