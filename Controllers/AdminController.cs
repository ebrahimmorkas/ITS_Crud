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

        [HttpGet]
        public IActionResult AddMumineen()
        {
            return View();
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
