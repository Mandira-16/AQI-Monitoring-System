using AQI_Monitoring_System.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AQI_Monitoring_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Here you would add your authentication logic
                // For example:
                // if (AuthenticateUser(model.Username, model.Password))
                // {
                //     // Set authentication cookie or token
                //     return RedirectToAction("Index");
                // }

                // For now, just redirect to home page after login attempt
                return RedirectToAction("Index");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
