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
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            List<FormModel> forms;
            if (User.IsInRole("Admin"))
            {
                forms = await _context.Forms.ToListAsync();
            }
            else
            {
                forms = await _context.Forms.Where(f => f.UserId == userId).ToListAsync();
            }

            return View(forms);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(FormModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                model.UserId = userId;
                model.DateFilled = DateTime.UtcNow;
                _context.Forms.Add(model);
                await _context.SaveChangesAsync();

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
        public async Task<IActionResult> Edit(int id)
        {
            var form = await _context.Forms.FindAsync(id);
            if (form == null || (form.UserId != _userManager.GetUserId(User) && !User.IsInRole("Admin")))
            {
                return Forbid();
            }
            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FormModel model)
        {
            if (ModelState.IsValid)
            {
                _context.Forms.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var form = await _context.Forms.FindAsync(id);
            if (form == null || (form.UserId != _userManager.GetUserId(User) && !User.IsInRole("Admin")))
            {
                return Forbid();
            }

            _context.Forms.Remove(form);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
