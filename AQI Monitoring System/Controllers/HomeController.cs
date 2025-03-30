using AQI_Monitoring_System.Data;
using AQI_Monitoring_System.Models;
using AQI_Monitoring_System.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace AQI_Monitoring_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;
        private readonly ISensorService _sensorService; 
        private readonly AqiSimulationService _simulationService;
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, IUserService userService,
            ISensorService sensorService, AqiSimulationService simulationService, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _userService = userService;
            _sensorService = sensorService;
            _simulationService = simulationService;
            _dbContext = dbContext;
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
            // Fetch all sensors for the "All Registered Sensors" table
            var sensors = _sensorService.GetAllSensors();
            ViewBag.ActiveSensorsCount = sensors?.Count(s => s.IsActive) ?? 0; // Null-safe
            ViewBag.SimulationStatus = _simulationService.IsRunning ? "Running" : "Stopped";

            // Fetch recent AQI readings (e.g., the latest reading for each sensor)
            var recentReadings = _dbContext.AqiReadings
                .Include(r => r.Sensor)
                .GroupBy(r => r.SensorId)
                .Select(g => g.OrderByDescending(r => r.RecordedAt).FirstOrDefault())
                .ToList();

            ViewBag.RecentReadings = recentReadings;
            return View(sensors);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Home");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult EditSensor(string id)
        {
            var sensor = _sensorService.GetSensorById(id);
            if (sensor == null)
            {
                return NotFound();
            }
            return View(sensor);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult StartSimulation()
        {
            _simulationService.StartSimulation();
            return RedirectToAction("MonitorAdminDashboard");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult StopSimulation()
        {
            _simulationService.StopSimulation();
            return RedirectToAction("MonitorAdminDashboard");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult AddSensor(string sensorId, string location, double latitude, double longitude)
        {
            // Basic validation
            if (string.IsNullOrEmpty(sensorId) || string.IsNullOrEmpty(location))
            {
                TempData["ErrorMessage"] = "Sensor ID and Location are required.";
                return RedirectToAction("MonitorAdminDashboard");
            }

            // Check if SensorId is unique (optional but recommended)
            if (_sensorService.GetAllSensors().Any(s => s.SensorId == sensorId))
            {
                TempData["ErrorMessage"] = "A sensor with this Sensor ID already exists.";
                return RedirectToAction("MonitorAdminDashboard");
            }

            // Create and add the sensor
            var sensor = new Sensor
            {
                SensorId = sensorId,
                Location = location,
                Latitude = latitude,
                Longitude = longitude,
                IsActive = true // New sensors start as active
            };

            _sensorService.AddSensor(sensor);
            TempData["SuccessMessage"] = $"Sensor {sensorId} added successfully!";
            return RedirectToAction("MonitorAdminDashboard");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult EditSensor(string sensorId, string location, double latitude, double longitude, bool isActive)
        {
            var sensor = _sensorService.GetSensorById(sensorId);
            if (sensor == null)
            {
                return NotFound();
            }

            // Update sensor properties
            sensor.Location = location;
            sensor.Latitude = latitude;
            sensor.Longitude = longitude;
            sensor.IsActive = isActive;

            _sensorService.UpdateSensor(sensor); // Assuming this method exists in ISensorService
            TempData["SuccessMessage"] = "Sensor updated successfully!";
            return RedirectToAction("MonitorAdminDashboard");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteSensor(string id)
        {
            var sensor = _sensorService.GetSensorById(id);
            if (sensor == null)
            {
                TempData["ErrorMessage"] = "Sensor not found.";
                return RedirectToAction("MonitorAdminDashboard");
            }

            _sensorService.DeleteSensor(id);
            TempData["SuccessMessage"] = $"Sensor {id} deleted successfully!";
            return RedirectToAction("MonitorAdminDashboard");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult DeactivateSensor(string id)
        {
            var sensor = _sensorService.GetSensorById(id);
            if (sensor == null)
            {
                TempData["ErrorMessage"] = "Sensor not found.";
                return RedirectToAction("MonitorAdminDashboard");
            }

            _sensorService.DeactivateSensor(id);
            TempData["SuccessMessage"] = $"Sensor {id} deactivated successfully!";
            return RedirectToAction("MonitorAdminDashboard");
        }

        // In HomeController.cs

        [Authorize(Roles = "Admin")]
        public IActionResult ManageUsers()
        {
            var users = _userService.GetAllUsers(); // Assuming this method exists in IUserService
            return View(users);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult EditUser(int id)
        {
            var user = _userService.GetUserById(id); // Assuming this method exists
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(int id, string username, string email, string role)
        {
            var user = _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            // Update user properties (password unchanged unless explicitly updated)
            user.Username = username;
            user.Email = email;
            user.Role = role;

            _userService.UpdateUser(user); // Assuming this method exists in IUserService
            TempData["SuccessMessage"] = "User updated successfully!";
            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUser(int id)
        {
            var user = _userService.GetUserById(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("ManageUsers");
            }

            _userService.DeleteUser(id); // Assuming this method exists in IUserService
            TempData["SuccessMessage"] = $"User {user.Username} deleted successfully!";
            return RedirectToAction("ManageUsers");
        }
    }
}