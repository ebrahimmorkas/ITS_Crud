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

        private bool IsAdminLoggedIn()
        {
            return HttpContext.Session.GetString("Role") == "admin";
        }

        [HttpGet]
        public IActionResult AddMumineen()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Auth");

            return View();
        }

        [HttpGet]
        public IActionResult UsersList()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Auth");

            // Fetch all users except admins
            var users = dbContext.Mumineen
                                 .Where(u => u.Role != "admin")
                                 .ToList();

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> AddMumineen(AddMumineenViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var student = new Mumineen
            {
                Id = Guid.NewGuid(),
                Its = model.Its,
                Name = model.Name,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = "user" // Admin adds normal users
            };

            await dbContext.Mumineen.AddAsync(student);
            await dbContext.SaveChangesAsync();

            ViewBag.Message = $"User {model.Name} added successfully!";
            return View();
        }
    }
}
