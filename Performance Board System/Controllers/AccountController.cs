using Microsoft.AspNetCore.Mvc;
using Performance_Board_System.DBContext;
using Performance_Board_System.Models;
using Performance_Board_System.Repository.Interfaces;

namespace Performance_Board_System.Controllers
{
    public class AccountController : Controller
    {
        private readonly DapperContext _context;
        private readonly IUserRepository _userRepository;

        public AccountController(DapperContext context, IUserRepository userRepository) {
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



    }
}
