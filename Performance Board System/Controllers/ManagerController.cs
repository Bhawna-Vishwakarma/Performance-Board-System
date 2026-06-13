using Microsoft.AspNetCore.Mvc;
using Performance_Board_System.Models;
using Performance_Board_System.Repository.Interfaces;
using System.Data;
using System.Reflection;

namespace Performance_Board_System.Controllers
{
    [Route("manager/[action]")]
    public class ManagerController : Controller
    {
        #region All Private properties

        private readonly IDepartmentRepository _deptRepo;
        private readonly IDesignationRepository _designationRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IRatingRepository _ratingRepo;
        private readonly IAttendanceStatusRepository _attendanceStatusRepo;
        private readonly IEvaluationRepository _evaluationRepo;
        private readonly IUserRepository _userRepo;

        #endregion


        #region Public Manager Constructor
        public ManagerController(IDepartmentRepository deptRepo, IDesignationRepository designationRepo, IRoleRepository roleRepo,
            IRatingRepository ratingRepo, IAttendanceStatusRepository attendanceStatusRepo, IEvaluationRepository evaluationRepo, IUserRepository userRepo)
        {
            _deptRepo = deptRepo;
            _designationRepo = designationRepo;
            _roleRepo = roleRepo;
            _ratingRepo = ratingRepo;
            _attendanceStatusRepo = attendanceStatusRepo;
            _evaluationRepo = evaluationRepo;
            _userRepo = userRepo;
        }
        #endregion


        #region Public Get Method for Manager Dashboard

        public async Task<IActionResult> ManagerDashboard()
        {
            var users = await _userRepo.GetAllActiveUsersAsync();
            var LatestFeedbacks = await _evaluationRepo.GetLatestFeedbacksAsync();

            ViewBag.TotalUsers = users.Count();
            ViewBag.LatestFeedbacks = LatestFeedbacks.OrderByDescending(e => e.EvaluationDate)
                                .Take(5)
                                .ToList();
            return View();
        }

        #endregion


        #region Get All Users

        [HttpGet("all-user")]
        public async Task<IActionResult> UserList()
        {
            var users = await _userRepo.GetAllActiveUsersAsync();
            return View(users);
        }

        #endregion


        #region Public Get method for Feedback
        
        [HttpGet]
        public IActionResult GiveFeedback(int userId)
        {
            var model = new EmployeeEvaluationViewModel
            {
                RatingList = _ratingRepo.GetAllAsync().Result.ToList(),
                EvaluatedUserId = userId,
                EvaluationPeriodStart = DateTime.Now.AddDays(-7),
                EvaluationPeriodEnd = DateTime.Now,
            };
            return View(model);
        }
        #endregion

        #region Public Post Method for Feedback
       
        [HttpPost]
        public async Task<IActionResult> GiveFeedback(EmployeeEvaluationViewModel model)
        {
            ModelState.Remove(nameof(model.RatingList));
            if (ModelState.IsValid)
            {
                string userIdString = HttpContext.Session.GetString("UserId");
                int.TryParse(userIdString, out int evaluatorId);
                var result = await _evaluationRepo.InsertEvaluationAsync(model, evaluatorId);
                
                (TempData["MessageType"], TempData["Message"]) = result switch
                {
                    1 => ("success", "Evaluation submitted successfully!"),
                    2 => ("success", "Evaluation updated successfully!"),
                    -1 => ("error", "Something went wrong while saving."),
                    _ => ("warning", "Unknown result from evaluation process.")
                };
                return RedirectToAction("FeedbackList", new { userId = model.EvaluatedUserId });
            }
            model.RatingList = (await _ratingRepo.GetAllAsync()).ToList();
            return View(model);
        }
        #endregion


        //public IActionResult FeedbackList()
        //{
        //    return View();
        //}

        public async Task<IActionResult> FeedbackList(int userId)
        {
            var evaluations = await _evaluationRepo.GetEvaluationsByUserIdAsync(userId);
            ViewBag.UserId = userId;
            return View(evaluations);
        }


        [HttpGet]
        public async Task<IActionResult> DeleteFeedback(int id, int evaluatedUserId)
        {
            var result = await _evaluationRepo.DeleteEvaluationAsync(id);

            (TempData["MessageType"], TempData["Message"]) = result switch
            {
                1 => ("success", "Feedback deleted successfully."),
                - 1 => ("error", "Something went wrong while deleting feedback."),
                _ => ("warning", "Unexpected result from deletion process.")
            };
            return RedirectToAction("FeedbackList", new { userId = evaluatedUserId });
        }
    }
}