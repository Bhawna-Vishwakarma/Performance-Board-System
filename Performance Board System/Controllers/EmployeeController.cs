using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Performance_Board_System.Models;
using Performance_Board_System.Repository.Interfaces;

namespace Performance_Board_System.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IAttendanceRepository _attendanceRepo;
        private readonly IEvaluationRepository _evaluationRepo;

        public EmployeeController(IAttendanceRepository attendanceRepository, IEvaluationRepository evaluationRepo)
        {
            _attendanceRepo = attendanceRepository;
            _evaluationRepo = evaluationRepo;
        }

        [HttpGet("employee-dahboard")]
        public async Task<IActionResult> EmployeeDashboard()
        {
            int userId = GetLoggedInUserId();

            ViewBag.OverallRating = await _evaluationRepo.GetAverageRatingByUserIdAsync(userId);
            var feedbacks = await _evaluationRepo.GetRecentFeedbacksAsync(userId);
            ViewBag.RecentFeedbacks = feedbacks.ToList();


            var LatestFeedbacks = await _evaluationRepo.GetLatestFeedbacksAsync();

            ViewBag.LatestFeedbacks = LatestFeedbacks.OrderByDescending(e => e.EvaluationDate)
                                .Take(5)
                                .ToList();

            var (avgScore, lastUpdated, categories) = await _evaluationRepo.GetUserRatingsAsync(userId);
            ViewBag.RatingCategories = categories;
            return View();
        }

        /// <summary>
        /// Get Employee Rating.
        /// </summary>
        /// <returns></returns>
        [Route("my-rating")]
        public async Task<IActionResult> Rating()
        {
            int userId = GetLoggedInUserId();
            var ratings = await _evaluationRepo.GetUserRatingsAsync(userId);
            ViewBag.AverageScore = await _evaluationRepo.GetAverageRatingByUserIdAsync(userId);
            ViewBag.LastUpdated = ratings.LastUpdated;
            ViewBag.Categories = ratings.Categories; // list of category + score
            return View();
        }

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

        [Route("attendance")]
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

        [Route("attendance-record")]
        public async Task<IActionResult> Attendance(DateTime? startDate, DateTime? endDate)
        {
            int userId = GetLoggedInUserId();

            // Default to current week if not filtered
            DateTime defaultStart = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1); // Monday
            DateTime defaultEnd = defaultStart.AddDays(4); // Friday

            DateTime from = startDate ?? defaultStart;
            DateTime to = endDate ?? defaultEnd;

            var records = await _attendanceRepo.GetAttendanceForUserInDateRange(userId, from, to);

            // Ensure full range is shown (even if days have no attendance)
            var allDates = Enumerable.Range(0, (to - from).Days + 1)
                                      .Select(i => from.AddDays(i))
                                      .Where(d => d.DayOfWeek != DayOfWeek.Sunday)
                                      .ToList();

            var viewModel = allDates.Select(date =>
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
                    IsCheckedIn = record != null && record.CheckInTime.HasValue
                };
            }).ToList();

            return View(viewModel);
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
            (var message, var type) = result switch
            {
                1 => ("Checked in successfully.", "success"),
                -1 => ("Already checked in today.", "warning"),
                _ => ("Something went wrong.", "danger")
            };

            TempData["Message"] = message;
            TempData["MessageType"] = type;
            return RedirectToAction("Attendance");
        }

        [HttpPost("attendance/checkout")]
        public async Task<IActionResult> CheckOut()
        {
            int userId = GetLoggedInUserId();

            int result = await _attendanceRepo.CheckOutAsync(userId);

            (var message, var type) = result switch
            {
                1 => ("Checked out successfully.", "success"),
                -1 => ("Check-in not found or already checked out.", "warning"),
                _ => ("Something went wrong.", "danger")
            };
            TempData["Message"] = message;
            TempData["MessageType"] = type;
            return RedirectToAction("Attendance");
        }



    }
}
