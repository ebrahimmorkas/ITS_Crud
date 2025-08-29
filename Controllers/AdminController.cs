using ITSAssignment.Web.Models;
using ITSAssignment.Web.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;

namespace ITSAssignment.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public AdminController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        private bool IsAdminLoggedIn() => HttpContext.Session.GetString("Role") == "admin";

        private IActionResult RedirectToLogin()
        {
            TempData["LoginMessage"] = "Please login to continue";
            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public IActionResult AddMumineen()
        {
            if (!IsAdminLoggedIn()) return RedirectToLogin();
            return View(new AdminAddMumineenViewModel()); // fresh model
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMumineen(AdminAddMumineenViewModel model)
        {
            if (!IsAdminLoggedIn()) return RedirectToLogin();

            if (!ModelState.IsValid) return View(model);

            if (dbContext.Mumineen.Any(x => x.Its == model.Its))
            {
                // ITS already exists → show error + blank fields
                ModelState.Clear(); // clear old errors
                ViewBag.Message = "ITS already added. Please try with a different ITS.";
                return View(new AdminAddMumineenViewModel());
            }

            var student = new Mumineen
            {
                Id = Guid.NewGuid(),
                Its = model.Its,
                Name = model.Name,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = "user"
            };

            await dbContext.Mumineen.AddAsync(student);
            await dbContext.SaveChangesAsync();

            ModelState.Clear();
            ViewBag.Message = $"User {model.Name} added successfully!";
            return View(new AdminAddMumineenViewModel()); // clear form after success
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!IsAdminLoggedIn()) return RedirectToLogin();

            var user = await dbContext.Mumineen.FindAsync(id);
            if (user == null)
            {
                TempData["Message"] = "User not found!";
                return RedirectToAction("UsersList");
            }

            if (user.Role == "admin")
            {
                TempData["Message"] = "Deleting admin user is not allowed.";
                return RedirectToAction("UsersList");
            }

            dbContext.Mumineen.Remove(user);
            await dbContext.SaveChangesAsync();

            TempData["Message"] = $"User with ITS {user.Its} deleted successfully!";
            return RedirectToAction("UsersList");
        }

        [HttpGet]
        public IActionResult UsersList()
        {
            if (!IsAdminLoggedIn()) return RedirectToLogin();

            var users = dbContext.Mumineen
                                 .Where(u => u.Role != "admin")
                                 .ToList();

            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(Guid id, string newPassword)
        {
            if (!IsAdminLoggedIn()) return RedirectToLogin();

            var user = await dbContext.Mumineen.FindAsync(id);
            if (user == null)
            {
                TempData["Message"] = "User not found!";
                return RedirectToAction("UsersList");
            }

            if (user.Role == "admin")
            {
                TempData["Message"] = "Admin password cannot be changed from here.";
                return RedirectToAction("UsersList");
            }

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                TempData["Message"] = "Password must be at least 6 characters.";
                return RedirectToAction("UsersList");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await dbContext.SaveChangesAsync();

            TempData["Message"] = $"Password for ITS {user.Its} changed successfully!";
            return RedirectToAction("UsersList");
        }
    }
}
