using CustomFormsApp.Data;
using CustomFormsApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomFormsApp.Controllers
{
    public class FormController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public FormController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager; // Add this line
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new FormModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submit(FormModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                model.UserId = userId;
                _context.Forms.Add(model);

                TempData["SuccessMessage"] = "Form submitted successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "There were validation errors.";
                return View("Index", model);
            }
        }

        [HttpGet]
        public IActionResult Result(FormModel model) 
        {
            return View(model);
        }
    }
}
