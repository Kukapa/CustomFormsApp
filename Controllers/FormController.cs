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
            ViewData["Name"] = name;
            ViewData["Email"] = email;
            return View("Result");
        }
    }
}
