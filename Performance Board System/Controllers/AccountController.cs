using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Performance_Board_System.DBContext;
using Performance_Board_System.Models;
using Performance_Board_System.Repository.Interfaces;
using System.Reflection;
using System.Security.Claims;

namespace Performance_Board_System.Controllers
{
    public class AccountController : Controller
    {
        #region Private Properties

        private readonly DapperContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IDepartmentRepository _deptRepo;
        private readonly IDesignationRepository _designationRepo;

        #endregion


        #region Constructor Method
        public AccountController(DapperContext context, IUserRepository userRepository, IDepartmentRepository deptRepo, IDesignationRepository designationRepo)
        {
            _context = context;
            _userRepository = userRepository;
            _deptRepo = deptRepo;
            _designationRepo = designationRepo;
        }

        #endregion


        #region public login Get Method 
        [HttpGet]
        [Route("login")]
        public IActionResult Login()
        {
            return View();
        }

        #endregion


        #region public signup Get Method

        [HttpGet]
        [Route("signup")]
        public async Task<IActionResult> SignUp()
        {
            var departments = await _deptRepo.GetAllAsync().ConfigureAwait(false);
            ViewBag.Departments = new SelectList(departments, "DepartmentID", "DepartmentName");
            var designations = await _designationRepo.GetAllAsync().ConfigureAwait(false);
            ViewBag.Designations = new SelectList(designations, "DesignationId", "Title");
            return View();
        }

        #endregion


        #region Public Post signup Method

        [HttpPost]
        [Route("signup")]
        public async Task<IActionResult> SignUp(User user)
        {
            if (ModelState.IsValid)
            {
                var result = await _userRepository.RegisterUser(user).ConfigureAwait(false); // Inject your repository via constructor

                if (result == 1)
                {
                    TempData["Message"] = "User registered successfully!";
                    TempData["MessageType"] = "success";
                    return RedirectToAction("login");
                }
                else if (result == -1)
                {
                    TempData["Message"] = "User already exists!";
                    TempData["MessageType"] = "warning";
                }
                else if (result == -2)
                {
                    TempData["Message"] = "User is not active!";
                    TempData["MessageType"] = "warning";
                }
                else
                {
                    TempData["Message"] = "User Not Registered";
                    TempData["MessageType"] = "danger";
                }
            }

            return View(user); // Return with validation messages or error toast
        }
        #endregion


        #region Public Post Login Method
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
                var result = await _userRepository.LoginUser(email, password).ConfigureAwait(false);
                switch (result)
                {
                    case 1:
                        var user = await _userRepository.GetUserByEmail(email.ToLower()).ConfigureAwait(false);
                        
                        if (user != null)
                        {
                            // Step 1: Create claims
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, email)
                                // Add more claims if needed (e.g., roles)
                            };

                            // Step 2: Create identity
                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                            // Step 3: Create auth properties (optional)
                            var authProperties = new AuthenticationProperties
                            {
                                IsPersistent = true, // persists cookie across browser sessions
                                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                            };

                            // Step 4: Sign in the user
                            await HttpContext.SignInAsync(
                                CookieAuthenticationDefaults.AuthenticationScheme,
                                new ClaimsPrincipal(claimsIdentity),
                                authProperties).ConfigureAwait(false);

                            // Store essential user info in session
                            HttpContext.Session.SetString("UserFullName", user.FullName);
                            HttpContext.Session.SetString("UserEmail", user.Email);
                            HttpContext.Session.SetString("UserId", user.UserId.ToString());
                            
                        HttpContext.Session.SetInt32("UserRole", user.RoleId);
                        }

                        TempData["Message"] = "Login successful!";
                        TempData["MessageType"] = "success";
                        int? roleId = HttpContext.Session.GetInt32("UserRole");

                        return roleId switch
                        {
                            1 => RedirectToAction("AdminDashboard", "Admin"),
                            2 => RedirectToAction("ManagerDashboard", "Manager"),
                            3 => RedirectToAction("EmployeeDashboard", "Employee"),
                            _ => RedirectToAction("Login", "Account")
                        };
                        //break;

                    case -1:
                        ModelState.AddModelError("", "Your account is inactive.");
                        TempData["Message"] = "Your account is inactive.";
                        TempData["MessageType"] = "danger";
                        break;

                    case -2:
                        ModelState.AddModelError("", "Incorrect email or password.");
                        TempData["Message"] = "Incorrect email or password.";
                        TempData["MessageType"] = "danger";
                        break;

                    default:
                        ModelState.AddModelError("", "An error occurred during login.");
                        TempData["Message"] = "Error Somthing Wrong";
                        TempData["MessageType"] = "danger";
                        break;
                }

            // return statement for all code paths
            return View();
        }

        #endregion


        #region public Get  logout Method

        [HttpGet]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            // Clear authentication cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);

            // Clear session data
            HttpContext.Session.Clear();
            TempData["Message"] = "Logged out successfully!";
            TempData["MessageType"] = "success";
            return RedirectToAction("Login");
        }
        #endregion

    }
}