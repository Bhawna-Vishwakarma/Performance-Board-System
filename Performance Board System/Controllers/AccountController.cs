using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Performance_Board_System.DBContext;
using Performance_Board_System.Models;
using Performance_Board_System.Repository.Interfaces;
using System.Reflection;
using System.Security.Claims;

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
                            HttpContext.Session.SetString("UserRole", user.Role);
                        }

                        TempData["Message"] = "Login successful!";
                        TempData["MessageType"] = "success";
                        return Redirect("dashboard");

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

            // Ensure a return statement for all code paths
            return View();
        }

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
    }

}
