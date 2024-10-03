using Microsoft.AspNetCore.Mvc;
using CustomFormsApp.Models;

namespace CustomFormsApp.Controllers
{
    public class TemplateController : Controller
    {
        [HttpGet]
        public IActionResult Create()
        {
            return View(new TemplateModel());
        }

        [HttpPost]
        public IActionResult Create(TemplateModel model)
        {
            if (ModelState.IsValid)
            {
                TempData["SuccessMessage"] = "Template created successfully!";
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
    }
}
