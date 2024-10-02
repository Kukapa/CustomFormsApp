using CustomFormsApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace CustomFormsApp.Controllers
{
    public class FormController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Submit(FormModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Result", model);
            }

            return View("Index", model);
        }

        public IActionResult Result(FormModel model)
        {
            return View(model);
        }
    }
}
