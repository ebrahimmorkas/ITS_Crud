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

        private bool IsUserLoggedIn() => !string.IsNullOrEmpty(HttpContext.Session.GetString("Its"));
        private IActionResult RedirectToLogin()
        {
            TempData["LoginMessage"] = "Please login to continue";
            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public IActionResult Add()
        {
            if (!IsUserLoggedIn()) return RedirectToLogin();

            int its = int.Parse(HttpContext.Session.GetString("Its")!);
            var user = dbContext.Mumineen.FirstOrDefault(u => u.Its == its);
            if (user == null) return RedirectToLogin();

            var model = new UserUpdateViewModel
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(UserUpdateViewModel model)
        {
            if (!IsUserLoggedIn()) return RedirectToLogin();
            if (!ModelState.IsValid) return View(model);

            int its = int.Parse(HttpContext.Session.GetString("Its")!);
            var user = dbContext.Mumineen.FirstOrDefault(u => u.Its == its);
            if (user == null) return RedirectToLogin();

            // Age < 16: email & phone can repeat, else must be unique
            if (model.Age >= 16)
            {
                if (dbContext.Mumineen.Any(u => u.Email_address == model.Email_address && u.Its != its))
                {
                    ModelState.AddModelError(nameof(model.Email_address), "Email already in use by another user.");
                    return View(model);
                }

                if (dbContext.Mumineen.Any(u => u.Mobile_number == model.Mobile_number && u.Its != its))
                {
                    ModelState.AddModelError(nameof(model.Mobile_number), "Mobile number already in use by another user.");
                    return View(model);
                }
            }

            // Save updates
            user.Name = model.Name;
            user.Age = model.Age;
            user.Gender = model.Gender;
            user.Mobile_number = model.Mobile_number;
            user.Email_address = model.Email_address;
            user.Marital_status = model.Marital_status;
            user.Address = model.Address;

            await dbContext.SaveChangesAsync();

            ViewBag.Message = "Profile updated successfully!";
            return View(model);
        }
    }
}
