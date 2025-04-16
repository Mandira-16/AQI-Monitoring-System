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
        private readonly ISimulationConfigService _simulationConfigService;

        public HomeController(ILogger<HomeController> logger, IUserService userService,
            ISensorService sensorService, AqiSimulationService simulationService, ApplicationDbContext dbContext, ISimulationConfigService simulationConfigService)
        {
            _logger = logger;
            _userService = userService;
            _sensorService = sensorService;
            _simulationService = simulationService;
            _dbContext = dbContext;
            _simulationConfigService = simulationConfigService;
        }

        public IActionResult Index()
        {
            try
            {
                _logger.LogInformation("Starting Index action.");
                // Retrieve the most recent reading for each active sensor
                var readings = _dbContext.AqiReadings
                    .Include(r => r.Sensor)
                    .Where(r => r.Sensor != null && r.Sensor.IsActive) // Only include readings from active sensors
                    .GroupBy(r => r.SensorId)
                    .Select(g => g.OrderByDescending(r => r.RecordedAt).FirstOrDefault())
                    .ToList();
                _logger.LogInformation($"Retrieved {readings.Count} readings in Index action.");

                // Retrieve thresholds for AQI color coding
                var thresholds = _dbContext.AlertThresholds.ToList();
                _logger.LogInformation($"Retrieved {thresholds.Count} thresholds in Index action.");

                // Ensure thresholds are not empty; if empty, seed default values
                if (!thresholds.Any())
                {
                    _logger.LogWarning("No thresholds found in AlertThresholds table. Seeding default thresholds.");
                    thresholds = new List<AlertThreshold>
            {
                new AlertThreshold { Category = "Good", MinAqi = 0, MaxAqi = 50, Color = "#009966" },
                new AlertThreshold { Category = "Moderate", MinAqi = 51, MaxAqi = 100, Color = "#FFDE33" },
                new AlertThreshold { Category = "Unhealthy for Sensitive Groups", MinAqi = 101, MaxAqi = 150, Color = "#FF9933" },
                new AlertThreshold { Category = "Unhealthy", MinAqi = 151, MaxAqi = 200, Color = "#CC0033" },
                new AlertThreshold { Category = "Very Unhealthy", MinAqi = 201, MaxAqi = 300, Color = "#660099" },
                new AlertThreshold { Category = "Hazardous", MinAqi = 301, MaxAqi = 500, Color = "#7E0023" }
            };
                    _dbContext.AlertThresholds.AddRange(thresholds);
                    _dbContext.SaveChanges();
                    thresholds = _dbContext.AlertThresholds.ToList();
                    _logger.LogInformation($"Seeded {thresholds.Count} default thresholds.");
                }

                // Retrieve all history for each active sensor, without time filtering
                var activeSensorIds = readings.Select(r => r.SensorId).ToList();
                var history = _dbContext.AqiReadings
                    .Where(r => activeSensorIds.Contains(r.SensorId))
                    .OrderBy(r => r.RecordedAt)
                    .ToList()
                    .GroupBy(r => r.SensorId)
                    .ToDictionary(
                        g => g.Key, // Key is the SensorId (e.g., "S001")
                        g => g.Select(r => new
                        {
                            recordedAt = r.RecordedAt.ToString("o"),
                            pm25 = r.Pm25 ?? 0.0,
                            pm10 = r.Pm10 ?? 0.0,
                            o3 = r.O3 ?? 0.0,
                            no2 = r.No2 ?? 0.0,
                            so2 = r.So2 ?? 0.0,
                            co = r.Co ?? 0.0
                        }).ToList()
                    );

                // Log the history data for debugging
                _logger.LogInformation("History Data:");
                foreach (var kvp in history)
                {
                    _logger.LogInformation($"Sensor {kvp.Key}: {kvp.Value.Count} readings");
                }

                // Prepare sensors data for the view
                var sensorsData = readings.Select(r => new
                {
                    sensorId = r.SensorId,
                    lat = r.Sensor != null ? r.Sensor.Latitude : 0.0,
                    lon = r.Sensor != null ? r.Sensor.Longitude : 0.0,
                    aqi = r.Aqi,
                    location = r.Sensor != null ? r.Sensor.Location : "Unknown",
                    recordedAt = r.RecordedAt.ToString("o"),
                    pm25 = r.Pm25 ?? 0.0,
                    pm10 = r.Pm10 ?? 0.0,
                    o3 = r.O3 ?? 0.0,
                    no2 = r.No2 ?? 0.0,
                    so2 = r.So2 ?? 0.0,
                    co = r.Co ?? 0.0
                }).ToList();

                // Pass data to the view
                ViewBag.Thresholds = thresholds;
                ViewBag.SensorsData = sensorsData;
                ViewBag.HistoryData = history; // Pass the dictionary directly
                _logger.LogInformation("Returning Index view.");
                return View(readings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Index action.");
                throw; // Re-throw the exception to see the error page (for debugging)
            }
        }

        public IActionResult SensorHistory(string sensorId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                _logger.LogInformation($"Starting SensorHistory action for SensorId: '{sensorId}', Start: {startDate}, End: {endDate}");

                // Validate sensorId
                if (string.IsNullOrWhiteSpace(sensorId))
                {
                    _logger.LogWarning("SensorId is null or empty.");
                    return BadRequest("Sensor ID is required.");
                }

                // Get the sensor
                var sensor = _dbContext.Sensors.FirstOrDefault(s => s.SensorId == sensorId);
                if (sensor == null)
                {
                    _logger.LogWarning($"Sensor with ID '{sensorId}' not found.");
                    return RedirectToAction("Index");
                }

                // Base query
                var query = _dbContext.AqiReadings.Where(r => r.SensorId == sensorId);

                // Apply date filters if provided
                if (startDate.HasValue)
                {
                    query = query.Where(r => r.RecordedAt >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    // Set end date to end of day
                    var endOfDay = endDate.Value.Date.AddDays(1).AddTicks(-1);
                    query = query.Where(r => r.RecordedAt <= endOfDay);
                }

                // Get the filtered history
                var history = query.OrderByDescending(r => r.RecordedAt).ToList();

                _logger.LogInformation($"Retrieved {history.Count} history records for SensorId '{sensorId}' in SensorHistory action.");

                ViewBag.SensorId = sensorId;
                ViewBag.Thresholds = _dbContext.AlertThresholds.ToList();
                ViewBag.Sensor = sensor;
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;

                return View(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in SensorHistory action for SensorId: '{sensorId}'");
                throw;
            }
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
                new Claim(ClaimTypes.Name, user.Username?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role?? "User")
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
            ViewBag.ActiveSensorsCount = sensors?.Count(s => s.IsActive) ?? 0;
            ViewBag.SimulationStatus = _simulationService.IsRunning ? "Running" : "Stopped";
            ViewBag.SimulationConfig = _simulationConfigService.GetConfig();

            // Fetch recent AQI readings
            var recentReadings = _dbContext.AqiReadings
                .Include(r => r.Sensor)
                .GroupBy(r => r.SensorId)
                .Select(g => g.OrderByDescending(r => r.RecordedAt).FirstOrDefault())
                .ToList();

            // Check for alerts
            var thresholds = _dbContext.AlertThresholds.ToList();
            ViewBag.Thresholds = thresholds;

            // Fetch historical data for each sensor (for the charts in the map popup)
            var historyData = new Dictionary<string, List<object>>();
            foreach (var reading in recentReadings)
            {
                if (reading != null && reading.Sensor != null && !historyData.ContainsKey(reading.SensorId))
                {
                    var sensorHistory = _dbContext.AqiReadings
                        .Where(r => r.SensorId == reading.SensorId)
                        .OrderByDescending(r => r.RecordedAt)
                        .Take(50) // Limit to the last 50 readings for performance
                        .Select(r => new
                        {
                            recordedAt = r.RecordedAt.ToString("o"),
                            pm25 = r.Pm25,
                            pm10 = r.Pm10,
                            o3 = r.O3,
                            no2 = r.No2,
                            so2 = r.So2,
                            co = r.Co
                        })
                        .AsEnumerable() // Switch to LINQ-to-Objects to cast to object
                        .Select(x => (object)x)
                        .ToList();
                    historyData[reading.SensorId] = sensorHistory;
                }
            }
            ViewBag.HistoryData = historyData; // Required for the map partial

            // Check for alerts
            var alerts = recentReadings
                .Where(r => r.Sensor != null && r.Sensor.IsActive)
                .Select(r => new
                {
                    SensorId = r.Sensor?.SensorId ?? "Unknown",
                    Aqi = r.Aqi,
                    Threshold = thresholds.FirstOrDefault(t => r.Aqi >= t.MinAqi && r.Aqi <= t.MaxAqi)
                })
                .Where(a => a.Threshold != null && a.Aqi > 100)
                .Select(a => $"Sensor {a.SensorId} has {(a.Threshold != null ? a.Threshold.Category : "Unknown")} air quality (AQI: {a.Aqi})")
                .ToList();

            ViewBag.RecentReadings = recentReadings;
            ViewBag.Alerts = alerts;
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

        // GET: /Home/ConfigureSimulation
        [Authorize(Roles = "Admin")]
        public IActionResult ConfigureSimulation()
        {
            var config = _simulationConfigService.GetConfig();
            return View(config);
        }

        // POST: /Home/ConfigureSimulation
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult ConfigureSimulation(SimulationConfig model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Validation for AQI
            if (model.FrequencyMinutes < 1 || model.FrequencyMinutes > 60)
            {
                ModelState.AddModelError("FrequencyMinutes", "Frequency must be between 1 and 60 minutes.");
                return View(model);
            }
            if (model.BaselineAqi < 0 || model.BaselineAqi > 500)
            {
                ModelState.AddModelError("BaselineAqi", "Baseline AQI must be between 0 and 500.");
                return View(model);
            }
            if (model.VariationRange < 0 || model.VariationRange > 500)
            {
                ModelState.AddModelError("VariationRange", "Variation range must be between 0 and 500.");
                return View(model);
            }
            // Validations for pollutant fields
            if (model.BaselinePm25 <= 0 || model.VariationPm25 <= 0)
            {
                ModelState.AddModelError("BaselinePm25", "PM2.5 baseline and variation must be greater than 0.");
                return View(model);
            }
            if (model.BaselinePm10 <= 0 || model.VariationPm10 <= 0)
            {
                ModelState.AddModelError("BaselinePm10", "PM10 baseline and variation must be greater than 0.");
                return View(model);
            }
            if (model.BaselineO3 <= 0 || model.VariationO3 <= 0)
            {
                ModelState.AddModelError("BaselineO3", "O3 baseline and variation must be greater than 0.");
                return View(model);
            }
            if (model.BaselineNo2 <= 0 || model.VariationNo2 <= 0)
            {
                ModelState.AddModelError("BaselineNo2", "NO2 baseline and variation must be greater than 0.");
                return View(model);
            }
            if (model.BaselineSo2 <= 0 || model.VariationSo2 <= 0)
            {
                ModelState.AddModelError("BaselineSo2", "SO2 baseline and variation must be greater than 0.");
                return View(model);
            }
            if (model.BaselineCo <= 0 || model.VariationCo <= 0)
            {
                ModelState.AddModelError("BaselineCo", "CO baseline and variation must be greater than 0.");
                return View(model);
            }

            _simulationConfigService.UpdateConfig(model);
            TempData["SuccessMessage"] = "Simulation configuration updated successfully!";
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
        public IActionResult ToggleSensorStatus(string id)
        {
            var sensor = _sensorService.GetSensorById(id);
            if (sensor == null)
            {
                TempData["ErrorMessage"] = "Sensor not found.";
                return RedirectToAction("MonitorAdminDashboard");
            }

            // Toggle the IsActive status
            sensor.IsActive = !sensor.IsActive;
            _sensorService.UpdateSensor(sensor); // Assuming UpdateSensor exists in ISensorService
            TempData["SuccessMessage"] = $"Sensor {id} {(sensor.IsActive ? "activated" : "deactivated")} successfully!";
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
        // GET: Display form to set thresholds (could be part of MonitorAdminDashboard or a separate page)
        [Authorize(Roles = "Admin")]
        public IActionResult ConfigureAlerts()
        {
            var thresholds = _dbContext.AlertThresholds.ToList();
            if (!thresholds.Any())
            {
                // Seed default thresholds with hex colors
                thresholds = new List<AlertThreshold>
        {
            new AlertThreshold { Category = "Good", MinAqi = 0, MaxAqi = 50, Color = "#009966" }, // Green
            new AlertThreshold { Category = "Moderate", MinAqi = 51, MaxAqi = 100, Color = "#FFDE33" }, // Yellow
            new AlertThreshold { Category = "Unhealthy for Sensitive Groups", MinAqi = 101, MaxAqi = 150, Color = "#FF9933" }, // Orange
            new AlertThreshold { Category = "Unhealthy", MinAqi = 151, MaxAqi = 200, Color = "#CC0033" }, // Red
            new AlertThreshold { Category = "Very Unhealthy", MinAqi = 201, MaxAqi = 300, Color = "#660099" }, // Purple
            new AlertThreshold { Category = "Hazardous", MinAqi = 301, MaxAqi = 500, Color = "#7E0023" } // Maroon
        };
                _dbContext.AlertThresholds.AddRange(thresholds);
                _dbContext.SaveChanges();
            }
            return View(thresholds);
        }

        // POST: Save updated thresholds
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult ConfigureAlerts(List<AlertThreshold> thresholds)
        {
            if (ModelState.IsValid)
            {
                foreach (var threshold in thresholds)
                {
                    var existing = _dbContext.AlertThresholds.Find(threshold.Id);
                    if (existing != null)
                    {
                        existing.Category = threshold.Category;
                        existing.MinAqi = threshold.MinAqi;
                        existing.MaxAqi = threshold.MaxAqi;
                        existing.Color = threshold.Color;
                        _dbContext.AlertThresholds.Update(existing);
                    }
                }
                _dbContext.SaveChanges();
                TempData["SuccessMessage"] = "Alert thresholds updated successfully!";
                return RedirectToAction("MonitorAdminDashboard");
            }
            return View(thresholds);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult PurgeOldReadings()
        {
            var cutoff = DateTime.UtcNow.AddMonths(-1); // Readings older than 1 month
            var oldReadings = _dbContext.AqiReadings.Where(r => r.RecordedAt < cutoff);
            int count = oldReadings.Count();
            if (count > 0)
            {
                _dbContext.AqiReadings.RemoveRange(oldReadings);
                _dbContext.SaveChanges();
                TempData["SuccessMessage"] = $"{count} old readings purged successfully.";
            }
            else
            {
                TempData["SuccessMessage"] = "No readings older than 1 month to purge.";
            }
            return RedirectToAction("MonitorAdminDashboard");
        }
    }
}