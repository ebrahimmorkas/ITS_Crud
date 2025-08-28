using ITSAssignment.Web.Models;
using ITSAssignment.Web.Models.Entities;
using Microsoft.AspNetCore.Mvc;

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

            int its = int.Parse(HttpContext.Session.GetString("Its")!);
            var user = dbContext.Mumineen.FirstOrDefault(u => u.Its == its);

            if (user == null) return RedirectToLogin();

            var model = new AddMumineenViewModel
            {
                Its = user.Its,
                Name = user.Name,
                Age = user.Age,
                Gender = user.Gender,
                Mobile_number = user.Mobile_number,
                Email_address = user.Email_address,
                Marital_status = user.Marital_status,
                Address = user.Address
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddMumineenViewModel viewModel)
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();

            int its = int.Parse(HttpContext.Session.GetString("Its")!);
            var user = dbContext.Mumineen.FirstOrDefault(u => u.Its == its);

            if (user == null) return RedirectToLogin();

            // Update fields (ITS should not change)
            user.Name = viewModel.Name;
            user.Age = viewModel.Age;
            user.Gender = viewModel.Gender;
            user.Mobile_number = viewModel.Mobile_number;
            user.Email_address = viewModel.Email_address;
            user.Marital_status = viewModel.Marital_status;
            user.Address = viewModel.Address;

            await dbContext.SaveChangesAsync();

            ViewBag.Message = "Profile updated successfully!";
            return View(viewModel);
        }
    }
}
