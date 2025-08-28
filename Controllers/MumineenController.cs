using ITSAssignment.Web.Models;
using ITSAssignment.Web.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;

namespace ITSAssignment.Web.Controllers
{
    public class MumineenController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        public MumineenController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        private bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("Its"));
        }

        private IActionResult RedirectToLogin()
        {
            TempData["LoginMessage"] = "Please login to continue";
            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public IActionResult Add()
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddMumineenViewModel viewModel)
        {
            var student = new Mumineen
            {
                Id = Guid.NewGuid(),
                Its = viewModel.Its,
                Name = viewModel.Name,
                Age = viewModel.Age,
                Gender = viewModel.Gender,
                Mobile_number = viewModel.Mobile_number,
                Marital_status = viewModel.Marital_status,
                Address = viewModel.Address,
                Password = BCrypt.Net.BCrypt.HashPassword(viewModel.Password)
            };

            await dbContext.Mumineen.AddAsync(student);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("Add");
        }
    }
}
