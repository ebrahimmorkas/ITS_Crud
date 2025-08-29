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
            // 🔥 Agar already login hai to login page mat dikhao
            var its = HttpContext.Session.GetString("Its");
            var role = HttpContext.Session.GetString("Role");

            if (!string.IsNullOrEmpty(its))
            {
                if (role == "admin")
                    return RedirectToAction("AddMumineen", "Admin");

                if (role == "user")
                    return RedirectToAction("Add", "Mumineen");
            }

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

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Its")))
            {
                TempData["LoginMessage"] = "Please login to continue";
                return RedirectToAction("Login");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword)
        {
            var its = HttpContext.Session.GetString("Its");
            if (string.IsNullOrEmpty(its))
                return RedirectToAction("Login");

            var user = dbContext.Mumineen.FirstOrDefault(u => u.Its.ToString() == its);
            if (user == null)
                return RedirectToAction("Login");

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.Password))
            {
                ViewBag.Message = "Current password is incorrect!";
                return View();
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await dbContext.SaveChangesAsync();

            ViewBag.Message = "Password changed successfully!";
            return View();
        }

    }
}