using ITSAssignment.Web.Models;
using ITSAssignment.Web.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net; // For hashing

namespace ITSAssignment.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public AuthController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            var user = dbContext.Mumineen.FirstOrDefault(u => u.Its == model.Its);

            if (user == null)
            {
                ViewBag.Message = "User not found!";
                return View(model);
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.Password);
            if (!isPasswordValid)
            {
                ViewBag.Message = "Incorrect password!";
                return View(model);
            }

            // Save login info in session
            HttpContext.Session.SetString("Its", user.Its.ToString());
            HttpContext.Session.SetString("Role", user.Role ?? "");

            // Redirect based on role
            if (user.Role == "admin")
                return RedirectToAction("AddMumineen", "Admin");

            return RedirectToAction("Add", "Mumineen");
        }
    }
}