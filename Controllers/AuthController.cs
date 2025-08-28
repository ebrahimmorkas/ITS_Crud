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
            // 1️⃣ Find user by ITS
            var user = dbContext.Mumineen.FirstOrDefault(u => u.Its == model.Its);

            if (user == null)
            {
                ViewBag.Message = "User not found!";
                return View(model);
            }

            // 2️⃣ Verify password
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.Password);
            if (!isPasswordValid)
            {
                ViewBag.Message = "Incorrect password!";
                return View(model);
            }

            // 3️⃣ Check role
            if (user.Role == "admin")
            {
                //ViewBag.Message = "Hello Admin!";
                return RedirectToAction("AddMumineen", "Admin");
            }
            else
            {
                ViewBag.Message = "Hello User!";
            }

            return View(model);
        }
    }
}