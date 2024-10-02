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
        public IActionResult Submit(string name, string email)
        {
            if (!ModelState.IsValid)
            {
                return View("Index");
            }

            ViewData["Name"] = name;
            ViewData["Email"] = email;
            return View("Result");
        }
    }
}
