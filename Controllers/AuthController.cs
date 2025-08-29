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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Convert ITS to string if needed
            string itsValue = model.Its.ToString(); // or model.Its if it's already string

            var user = dbContext.Mumineen.FirstOrDefault(u => u.Its.ToString() == itsValue);

            // Generic error message for all failures
            if (user == null || string.IsNullOrEmpty(user.Password) ||
                !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                ViewBag.Message = "Invalid credentials.";
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