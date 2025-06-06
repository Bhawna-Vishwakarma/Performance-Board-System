using Microsoft.AspNetCore.Mvc;
using Performance_Board_System.Models;
using Performance_Board_System.Repository.Implementations;
using Performance_Board_System.Repository.Interfaces;

namespace Performance_Board_System.Controllers
{
    public class AdminController : Controller
    {

        #region All Private properties

        private readonly IDepartmentRepository _deptRepo;
        private readonly IDesignationRepository _designationRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IRatingRepository _ratingRepo;
        private readonly IAttendanceStatusRepository _attendanceStatusRepo;

        #endregion


        #region public Constructor
        public AdminController(IDepartmentRepository deptRepo, IDesignationRepository designationRepo, IRoleRepository roleRepo,
            IRatingRepository ratingRepo, IAttendanceStatusRepository attendanceStatusRepo)
        {
            _deptRepo = deptRepo;
            _designationRepo = designationRepo;
            _roleRepo = roleRepo;
            _ratingRepo = ratingRepo;
            _attendanceStatusRepo = attendanceStatusRepo;
        }
        #endregion


        #region Public Dashbord Methods

        [HttpGet("admin-dashboard")]
        public IActionResult AdminDashboard()
        {
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
            await _deptRepo.AddAsync(dept).ConfigureAwait(false);
            TempData["Message"] = "Department added successfully.";
            TempData["MessageType"] = "success";
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
            await _deptRepo.UpdateAsync(dept).ConfigureAwait(false);
            TempData["Message"] = "Department updated successfully.";
            TempData["MessageType"] = "success";
            return RedirectToAction("DepartmentList");
        }

        [HttpPost("delete-department/{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            await _deptRepo.DeleteAsync(id).ConfigureAwait(false);
            TempData["Message"] = "Department deleted.";
            TempData["MessageType"] = "danger";
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
            if (result == 1)
            {
                TempData["Message"] = "Designation added successfully.";
                TempData["MessageType"] = "success";
                return RedirectToAction("DesignationList");
            }
            else if( result == -1)
            {
                TempData["Message"] = "Designation Alredy Exists.";
                TempData["MessageType"] = "warning";
            }
            else
            {
                TempData["Message"] = "Someting Wrong.";
                TempData["MessageType"] = "danger";
            }
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
            await _designationRepo.UpdateAsync(designation).ConfigureAwait(false);
            TempData["Message"] = "Designation updated successfully.";
            TempData["MessageType"] = "success";
            return RedirectToAction("DesignationList");
        }

        [HttpPost("delete-designation/{id}")]
        public async Task<IActionResult> DeleteDesignation(int id)
        {
            await _designationRepo.DeleteAsync(id).ConfigureAwait(false);
            TempData["Message"] = "Designation deleted.";
            TempData["MessageType"] = "danger";
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
            await _roleRepo.AddAsync(role).ConfigureAwait(false);
            TempData["Message"] = "Role added successfully.";
            TempData["MessageType"] = "success";
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
            await _roleRepo.UpdateAsync(role).ConfigureAwait(false);
            TempData["Message"] = "Role updated successfully.";
            TempData["MessageType"] = "success";
            return RedirectToAction("RoleList");
        }

        [HttpPost("delete-role/{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            await _roleRepo.DeleteAsync(id).ConfigureAwait(false);
            TempData["Message"] = "Role deleted.";
            TempData["MessageType"] = "danger";
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
            await _ratingRepo.AddAsync(rating).ConfigureAwait(false);
            TempData["Message"] = "Rating added successfully.";
            TempData["MessageType"] = "success";
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
            await _ratingRepo.UpdateAsync(rating).ConfigureAwait(false);
            TempData["Message"] = "Rating updated successfully.";
            TempData["MessageType"] = "success";
            return RedirectToAction("RatingList");
        }

        [HttpPost("delete-rating/{id}")]
        public async Task<IActionResult> DeleteRating(int id)
        {
            await _ratingRepo.DeleteAsync(id).ConfigureAwait(false);
            TempData["Message"] = "Rating deleted.";
            TempData["MessageType"] = "danger";
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
            await _attendanceStatusRepo.AddAsync(status).ConfigureAwait(false);
            TempData["Message"] = "Attendance status added successfully.";
            TempData["MessageType"] = "success";
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
            await _attendanceStatusRepo.UpdateAsync(status).ConfigureAwait(false);
            TempData["Message"] = "Attendance status updated successfully.";
            TempData["MessageType"] = "success";
            return RedirectToAction("AttendanceStatusList");
        }

        [HttpPost("delete-attendance-status/{id}")]
        public async Task<IActionResult> DeleteAttendanceStatus(int id)
        {
            await _attendanceStatusRepo.DeleteAsync(id).ConfigureAwait(false);
            TempData["Message"] = "Attendance status deleted.";
            TempData["MessageType"] = "danger";
            return RedirectToAction("AttendanceStatusList");
        }


        #endregion

    }
}
