using CustomFormsApp.Data;
using CustomFormsApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomFormsApp.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Results(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View(new List<TemplateModel>());
            }

            var templates = await _context.Templates
                .Where(t => t.Title.Contains(query) || t.Description.Contains(query) ||
                            t.Questions.Any(q => q.Title.Contains(query)) || 
                            t.Comments.Any(c => c.Content.Contains(query)))
                .Include(t => t.Questions)
                .ToListAsync();

            return View(templates);
        }
    }
}
