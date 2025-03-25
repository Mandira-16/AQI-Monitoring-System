using AQI_Monitoring_System.Models;
using AQI_Monitoring_System.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AQI_Monitoring_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;

        public HomeController(ILogger<HomeController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        // POST: /Home/Login
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            // Clear any previous error messages
            ViewData["UsernameError"] = null;
            ViewData["PasswordError"] = null;

            // Server-side validation
            if (string.IsNullOrEmpty(model.Username))
            {
                ViewData["UsernameError"] = "Username is required";
                ModelState.AddModelError("Username", "Username is required");
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                ViewData["PasswordError"] = "Password is required";
                ModelState.AddModelError("Password", "Password is required");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if username exists and if password is correct
            var user = _userService.GetUserByUsername(model.Username);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Invalid username or password";
                return View(model);
            }

            // Verify password
            bool isPasswordValid = _userService.VerifyPassword(user, model.Password);
            if (!isPasswordValid)
            {
                TempData["ErrorMessage"] = "Invalid username or password";
                return View(model);
            }

            // Login successful - create authentication cookie, etc.
            // For a real implementation, you would use something like:
            // HttpContext.SignInAsync(...)

            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}