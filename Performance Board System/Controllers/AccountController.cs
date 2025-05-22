using Microsoft.AspNetCore.Mvc;
using Performance_Board_System.DBContext;
using Performance_Board_System.Models;
using Performance_Board_System.Repository.Interfaces;
using System.Reflection;

namespace Performance_Board_System.Controllers
{
    public class AccountController : Controller
    {
        private readonly DapperContext _context;
        private readonly IUserRepository _userRepository;

        public AccountController(DapperContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        [Route("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        [Route("signup")]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [Route("signup")]
        public async Task<IActionResult> SignUp(User user)
        {
            if (ModelState.IsValid)
            {
                var result = await _userRepository.RegisterUser(user); // Inject your repository via constructor

                if (result == 1)
                {
                    TempData["Message"] = "User registered successfully!";
                    TempData["MessageType"] = "success";
                    return RedirectToAction("Login");
                }
                else if (result == -1)
                {
                    TempData["Message"] = "User already exists!";
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

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
                var result = await _userRepository.LoginUser(email, password);
                switch (result)
                {
                    case 1:
                        var user = await _userRepository.GetUserByEmail(email.ToLower());

                        if (user != null)
                        {
                            // Store essential user info in session
                            HttpContext.Session.SetString("UserFullName", user.FullName);
                            HttpContext.Session.SetString("UserEmail", user.Email);
                            HttpContext.Session.SetString("UserRole", user.Role);
                        }

                        TempData["Message"] = "Login successful!";
                        TempData["MessageType"] = "success";
                        return RedirectToAction("Index", "Home");

                    case -1:
                        ModelState.AddModelError("", "Your account is inactive.");
                        TempData["Message"] = "Your account is inactive.";
                        TempData["MessageType"] = "danger";
                        break;

                    case -2:
                        ModelState.AddModelError("", "Incorrect email or password.");
                        break;

                    default:
                        ModelState.AddModelError("", "An error occurred during login.");
                        break;
                }

            // Ensure a return statement for all code paths
            return View();
        }

        [HttpGet]
        [Route("logout")]
        public IActionResult Logout()
        {
            // Clear session data
            HttpContext.Session.Clear();
            TempData["Message"] = "Logged out successfully!";
            TempData["MessageType"] = "success";
            return RedirectToAction("Login");
        }
    }

}
