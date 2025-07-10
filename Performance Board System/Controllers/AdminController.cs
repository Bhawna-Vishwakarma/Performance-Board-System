using Microsoft.AspNetCore.Mvc;
using Performance_Board_System.Models;
using Performance_Board_System.Repository.Implementations;
using Performance_Board_System.Repository.Interfaces;
using System.Linq;

namespace Performance_Board_System.Controllers
{
    [Route("admin/[action]")]
    public class AdminController : Controller
    {

        #region All Private properties

        private readonly IDepartmentRepository _deptRepo;
        private readonly IDesignationRepository _designationRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IRatingRepository _ratingRepo;
        private readonly IAttendanceStatusRepository _attendanceStatusRepo;
        private readonly IUserRepository _userRepo;
        private readonly IEvaluationRepository _evaluationRepo;

        #endregion


        #region public Constructor
        public AdminController(IDepartmentRepository deptRepo, IDesignationRepository designationRepo, IRoleRepository roleRepo,
            IRatingRepository ratingRepo, IAttendanceStatusRepository attendanceStatusRepo, IUserRepository userRepo, IEvaluationRepository evaluationRepo)
        {
            _deptRepo = deptRepo;
            _designationRepo = designationRepo;
            _roleRepo = roleRepo;
            _ratingRepo = ratingRepo;
            _attendanceStatusRepo = attendanceStatusRepo;
            _userRepo = userRepo;
            _evaluationRepo = evaluationRepo;
        }
        #endregion


        #region Public Get Dashbord Methods

        [HttpGet("admin-dashboard")]
        public async Task<IActionResult> AdminDashboard()
        {
            var users = await _userRepo.GetAllActiveUsersAsync();
            var departments = await _deptRepo.GetAllAsync();
            var designations = await _designationRepo.GetAllAsync();
            var roles = await _roleRepo.GetAllAsync();
            var ratings = await _ratingRepo.GetAllAsync();
            var attendanceStatus = await _attendanceStatusRepo.GetAllAsync();
            var LatestFeedbacks = await _evaluationRepo.GetLatestFeedbacksAsync();

            ViewBag.TotalUsers = users.Count();
            ViewBag.TotalDepartments = departments.Count();
            ViewBag.TotalDesignations = designations.Count();
            ViewBag.TotalRoles = roles.Count();
            ViewBag.TotalRatings = ratings.Count();
            ViewBag.TotalAttendanceStatus = attendanceStatus.Count();
            ViewBag.LatestFeedbacks = LatestFeedbacks.OrderByDescending(e => e.EvaluationDate)
                                .Take(5)
                                .ToList();
            return View();
        }
        #endregion


        #region Department Methos

        [HttpGet("departments")]
        public async Task<IActionResult> DepartmentList()
        {
            var departments = await _deptRepo.GetAllAsync().ConfigureAwait(false);
            return View(departments);
        }

        [HttpGet("create-department")]
        public IActionResult CreateDepartment() => View();

        [HttpPost("create-department")]
        public async Task<IActionResult> CreateDepartment(Department dept)
        {
            if (!ModelState.IsValid) return View(dept);
            var result = await _deptRepo.AddAsync(dept).ConfigureAwait(false);
            (var message, var type) = result switch
            {
                1 => ("Department added successfully.", "success"),
                -1 => ("Department Already Exists.", "warning"),
                _ => ("Something went wrong.", "danger")
            };

            TempData["Message"] = message;
            TempData["MessageType"] = type;
            return RedirectToAction("DepartmentList");
        }

        [HttpGet("edit-department")]
        public async Task<IActionResult> EditDepartment(int id)
        {
            var dept = await _deptRepo.GetByIdAsync(id).ConfigureAwait(false);
            if (dept == null) return NotFound();
            return View(dept);
        }

        [HttpPost("edit-department")]
        public async Task<IActionResult> EditDepartment(Department dept)
        {
            if (!ModelState.IsValid) return View(dept);
            var result = await _deptRepo.UpdateAsync(dept).ConfigureAwait(false);
            (var message, var type) = result switch
            {
                1 => ("Department updated successfully.", "success"),
                -1 => ("Department already exists.", "warning"),
                -2 => ("Department not found.", "danger"),
                _ => ("Something went wrong.", "danger")
            };

            TempData["Message"] = message;
            TempData["MessageType"] = type;
            return RedirectToAction("DepartmentList");
        }

        [HttpPost("delete-department/{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var result = await _deptRepo.DeleteAsync(id).ConfigureAwait(false);
            (var message, var type) = result switch
            {
                1 => ("Department deleted successfully.", "success"),
                -1 => ("Department not found.", "warning"),
                0 => ("Something went wrong during deletion.", "danger"),
                _ => ("Unexpected error occurred.", "danger")
            };

            TempData["Message"] = message;
            TempData["MessageType"] = type;
            return RedirectToAction("DepartmentList");
        }

        #endregion


        #region Designation Methods

        [HttpGet("designations")]
        public async Task<IActionResult> DesignationList()
        {
            var designations = await _designationRepo.GetAllAsync().ConfigureAwait(false);
            return View(designations);
        }

        [HttpGet("create-designation")]
        public IActionResult CreateDesignation() => View();

        [HttpPost("create-designation")]
        public async Task<IActionResult> CreateDesignation(Designation designation)
        {
            if (!ModelState.IsValid) return View(designation);
            var result = await _designationRepo.AddAsync(designation).ConfigureAwait(false);
            (var message, var type) = result switch
            {
                1 => ("Designation added successfully.", "success"),
                -1 => ("Designation Already Exists.", "warning"),
                _ => ("Something went wrong.", "danger")
            };

            TempData["Message"] = message;
            TempData["MessageType"] = type;
            return RedirectToAction("DesignationList");
        }

        [HttpGet("edit-designation")]
        public async Task<IActionResult> EditDesignation(int id)
        {
            var designation = await _designationRepo.GetByIdAsync(id).ConfigureAwait(false);
            if (designation == null) return NotFound();
            return View(designation);
        }

        [HttpPost("edit-designation")]
        public async Task<IActionResult> EditDesignation(Designation designation)
        {
            if (!ModelState.IsValid) return View(designation);
            var result = await _designationRepo.UpdateAsync(designation).ConfigureAwait(false);
            (var message, var type) = result switch
            {
                1 => ("Designation updated successfully.", "success"),
                -1 => ("Designation already exists.", "warning"),
                -2 => ("Designation not found.", "danger"),
                _ => ("Something went wrong.", "danger")
            };

            TempData["Message"] = message;
            TempData["MessageType"] = type;

            return RedirectToAction("DesignationList");

        }


        [HttpPost("delete-designation/{id}")]
        public async Task<IActionResult> DeleteDesignation(int id)
        {
            var result = await _designationRepo.DeleteAsync(id).ConfigureAwait(false);
            (var message, var type) = result switch
            {
                1 => ("Designation deleted successfully.", "success"),
                -1 => ("Designation not found.", "warning"),
                0 => ("Something went wrong during deletion.", "danger"),
                _ => ("Unexpected error occurred.", "danger")
            };

            TempData["Message"] = message;
            TempData["MessageType"] = type;

            return RedirectToAction("DesignationList");
        }

        #endregion


        #region Role Methods
        
        [HttpGet("roles")]
        public async Task<IActionResult> RoleList()
        {
            var roles = await _roleRepo.GetAllAsync().ConfigureAwait(false);
            return View(roles);
        }

        [HttpGet("create-role")]
        public IActionResult CreateRole() => View();

        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRole(Role role)
        {
            if (!ModelState.IsValid) return View(role);
            var result = await _roleRepo.AddAsync(role).ConfigureAwait(false);
            (var message, var type) = result switch
            {
                1 => ("Role added successfully.", "success"),
                -1 => ("Role Already Exists.", "warning"),
                _ => ("Something went wrong.", "danger")
            };

            TempData["Message"] = message;
            TempData["MessageType"] = type;
            return RedirectToAction("RoleList");
        }

        [HttpGet("edit-role")]
        public async Task<IActionResult> EditRole(int id)
        {
            var role = await _roleRepo.GetByIdAsync(id).ConfigureAwait(false);
            if (role == null) return NotFound();
            return View(role);
        }

        [HttpPost("edit-role")]
        public async Task<IActionResult> EditRole(Role role)
        {
            if (!ModelState.IsValid) return View(role);
            var result = await _roleRepo.UpdateAsync(role).ConfigureAwait(false);
            (var message, var type) = result switch
            {
                1 => ("Role updated successfully.", "success"),
                -1 => ("Role already exists.", "warning"),
                -2 => ("Role not found.", "danger"),
                _ => ("Something went wrong.", "danger")
            };

            TempData["Message"] = message;
            TempData["MessageType"] = type;
            return RedirectToAction("RoleList");
        }

        [HttpPost("delete-role/{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var result = await _roleRepo.DeleteAsync(id).ConfigureAwait(false);
            (var message, var type) = result switch
            {
                1 => ("Role deleted successfully.", "success"),
                -1 => ("Role not found.", "warning"),
                0 => ("Something went wrong during deletion.", "danger"),
                _ => ("Unexpected error occurred.", "danger")
            };

            TempData["Message"] = message;
            TempData["MessageType"] = type;
            return RedirectToAction("RoleList");
        }
        #endregion


        #region Rating Methods

        [HttpGet("ratings")]
        public async Task<IActionResult> RatingList()
        {
            var ratings = await _ratingRepo.GetAllAsync().ConfigureAwait(false);
            return View(ratings);
        }

        [HttpGet("create-rating")]
        public IActionResult CreateRating() => View();

        [HttpPost("create-rating")]
        public async Task<IActionResult> CreateRating(Rating rating)
        {
            if (!ModelState.IsValid) return View(rating);
            var result = await _ratingRepo.AddAsync(rating).ConfigureAwait(false);
            (var message, var type) = result switch
            {
                1 => ("Rating added successfully.", "success"),
                -1 => ("Rating Already Exists.", "warning"),
                _ => ("Something went wrong.", "danger")
            };

            TempData["Message"] = message;
            TempData["MessageType"] = type;
            return RedirectToAction("RatingList");
        }

        [HttpGet("edit-rating")]
        public async Task<IActionResult> EditRating(int id)
        {
            var rating = await _ratingRepo.GetByIdAsync(id).ConfigureAwait(false);
            if (rating == null) return NotFound();
            return View(rating);
        }

        [HttpPost("edit-rating")]
        public async Task<IActionResult> EditRating(Rating rating)
        {
            if (!ModelState.IsValid) return View(rating);
            var result = await _ratingRepo.UpdateAsync(rating).ConfigureAwait(false);
            (var message, var type) = result switch
            {
                1 => ("Rating Updated successfully", "success"),
                -1 => ("Duplicate Entries Not Allowed.", "info"),
                -2 => ("Rating not found.", "danger"),
                _ => ("Something went wrong.", "danger")
            };

            TempData["Message"] = message;
            TempData["MessageType"] = type;

            return RedirectToAction("RatingList");
        }

        [HttpPost("delete-rating/{id}")]
        public async Task<IActionResult> DeleteRating(int id)
        {
            var result = await _ratingRepo.DeleteAsync(id).ConfigureAwait(false);
            (var message, var type) = result switch
            {
                1 => ("Rating Updated successfully", "success"),
                -1 => ("Rating not found.", "danger"),
                _ => ("Something went wrong.", "danger")
            };

            TempData["Message"] = message;
            TempData["MessageType"] = type;
            return RedirectToAction("RatingList");
        }

        #endregion


        #region Attandance Status Methods
        
        [HttpGet("attendance-statuses")]
        public async Task<IActionResult> AttendanceStatusList()
        {
            var statuses = await _attendanceStatusRepo.GetAllAsync().ConfigureAwait(false);
            return View(statuses);
        }

        [HttpGet("create-attendance-status")]
        public IActionResult CreateAttendanceStatus() => View();

        
        
        
        [HttpPost("create-attendance-status")]

        public async Task<IActionResult> CreateAttendanceStatus(AttendanceStatusMaster status)
        {
            if (!ModelState.IsValid) return View(status);
            var result = await _attendanceStatusRepo.AddAsync(status).ConfigureAwait(false);
            (var message, var type) = result switch
            {
                1 => ("AttendanceStatus added successfully.", "success"),
                -1 => ("AttendanceStatus Already Exists.", "warning"),
                _ => ("Something went wrong.", "danger")
            };

            TempData["Message"] = message;
            TempData["MessageType"] = type;
            return RedirectToAction("AttendanceStatusList");
        }


        [HttpGet("edit-attendance-status")]
        public async Task<IActionResult> EditAttendanceStatus(int id)
        {
            var status = await _attendanceStatusRepo.GetByIdAsync(id).ConfigureAwait(false);
            if (status == null) return NotFound();
            return View(status);
        }

        [HttpPost("edit-attendance-status")]
        public async Task<IActionResult> EditAttendanceStatus(AttendanceStatusMaster status)
        {
            if (!ModelState.IsValid) return View(status);
            var result = await _attendanceStatusRepo.UpdateAsync(status).ConfigureAwait(false);
            (var message, var type) = result switch
            {
                1 => ("Attendance Status updated successfully.", "success"),
                -1 => ("Attendance Status already exists.", "warning"),
                -2 => ("Attendance Status not found.", "danger"),
                _ => ("Something went wrong.", "danger")
            };

            TempData["Message"] = message;
            TempData["MessageType"] = type;
            return RedirectToAction("AttendanceStatusList");
        }

        [HttpPost("delete-attendance-status/{id}")]
        public async Task<IActionResult> DeleteAttendanceStatus(int id)
        {
            var result = await _attendanceStatusRepo.DeleteAsync(id).ConfigureAwait(false);
            (var message, var type) = result switch
            {
                1 => ("Attendance Status deleted successfully.", "success"),
                -1 => ("Attendance Status not found.", "warning"),
                0 => ("Something went wrong during deletion.", "danger"),
                _ => ("Unexpected error occurred.", "danger")
            };

            TempData["Message"] = message;
            TempData["MessageType"] = type;
            return RedirectToAction("AttendanceStatusList");
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


        #region Get Method for assign user Role Designation and Role
        [HttpGet("assign-role")]
        public async Task<IActionResult> AssignRole(int id)
        {
            var user = await _userRepo.GetUserById(id);
            if (user == null) return NotFound();

            var viewModel = new UserRolesAssignViewModel
            {
                UserId = user.UserId,
                FullName = user.FullName,
                DepartmentID = user.DepartmentID,
                DesignationId = user.DesignationId,
                RoleId = user.RoleId,
                Department = (await _deptRepo.GetAllAsync()).ToList(),
                Designation = (await _designationRepo.GetAllAsync()).ToList(),
                Role = (await _roleRepo.GetAllAsync()).ToList()
            };

            return View(viewModel);
        }
        #endregion


        #region Post Method for assign user Role Designation and Role

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole(UserRolesAssignViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Repopulate dropdowns in case of validation error
                //model.Department = (List<Department>)await _deptRepo.GetAllAsync();
                //model.Designation = (List<Designation>)await _designationRepo.GetAllAsync();
                //model.Role = (List<Role>)await _roleRepo.GetAllAsync();
                return View(model);
            }

            var user = new User
            {
                UserId = model.UserId,
                FullName = model.FullName,
                DepartmentID = model.DepartmentID,
                DesignationId = model.DesignationId,
                RoleId = model.RoleId
            };

            var result = await _userRepo.UpdateUserRoleAsync(user).ConfigureAwait(false);

            (var message, var type) = result switch
            {
                1 => ("User role assigned successfully.", "success"),
                -1 => ("User not found.", "warning"),
                _ => ("Something went wrong.", "danger")
            };

            TempData["Message"] = message;
            TempData["MessageType"] = type;

            return RedirectToAction("UserList");
        }




        #endregion


        #region Delete User Method

        [HttpPost("delete-user/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userRepo.SoftDeleteUserAsync(id).ConfigureAwait(false);
            (var message, var type) = result switch
            {
                1 => ("User deleted successfully.", "success"),
                -1 => ("User not found.", "warning"),
                _ => ("Something went wrong during deletion.", "danger")
            };
            TempData["Message"] = message;
            TempData["MessageType"] = type;
            return RedirectToAction("UserList");
        }
        #endregion


        #region Add User Method For activate user

        [HttpPost("add-user")]
        public async Task<IActionResult> AddUser(int id)
        { 
            var result = await _userRepo.AddUser(id).ConfigureAwait(false);
            TempData["Message"] = "User Activated Successfully";
            TempData["MessageType"] = "success";
            return RedirectToAction("UserList");

        }
        #endregion


        #region Get Method for FeedbackList
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
                -1 => ("error", "Something went wrong while deleting feedback."),
                _ => ("warning", "Unexpected result from deletion process.")
            };
            return RedirectToAction("FeedbackList", new { userId = evaluatedUserId });
        }

        #endregion
    }
}
