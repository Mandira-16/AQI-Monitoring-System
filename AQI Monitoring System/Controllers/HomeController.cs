using AQI_Monitoring_System.Models;
using AQI_Monitoring_System.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

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
            ViewData["ActivePage"] = "Home";
            return View();
        }

        public IActionResult Privacy()
        {
            ViewData["ActivePage"] = "Privacy";
            return View();
        }
        
        public IActionResult Login()
        {
            ViewData["ActivePage"] = "Login";
            return View();
        }

        // POST: /Home/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
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

            // Add role to claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Redirect based on role
            if (user.Role == "Admin")
            {
                return RedirectToAction("MonitorAdminDashboard", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home"); // For now, redirect non-Admins to the Index page
            }
        }


        // GET: /Home/Register
        [Authorize(Roles = "Admin")] // Restrict to Admins only
        public IActionResult Register()
        {
            _logger.LogInformation("Register GET action called"); // Added this line to test
            return View(new RegisterViewModel());
        }

        // POST: /Home/Register
        [HttpPost]
        [Authorize(Roles = "Admin")] // Restrict to Admins only
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if username already exists
            if (_userService.UsernameExists(model.Username))
            {
                ModelState.AddModelError("Username", "Username is already taken");
                return View(model);
            }

            // Create new user (assuming _userService has a method to add a user)
            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                // Password should be hashed before saving
                PasswordHash = _userService.HashPassword(model.Password),
                Role = model.Role
            };

            _userService.AddUser(user); // Assuming you have this method in IUserService

            TempData["SuccessMessage"] = "Registration successful! Please log in.";
            return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        public IActionResult CheckUsername(string username)
        {
            var exists = _userService.UsernameExists(username);
            return Json(new { exists = exists });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult MonitorAdminDashboard()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Home");
        }
    }
}