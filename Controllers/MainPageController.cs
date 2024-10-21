using Microsoft.AspNetCore.Mvc;
using CustomFormsApp.Data;
using CustomFormsApp.Models;

namespace CustomFormsApp.Controllers
{
    public class MainPageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MainPageController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var latestTemplates = _context.Templates
                .OrderByDescending(t => t.CreatedDate)
                .Take(10)
                .ToList();

            var popularTemplates = _context.Templates
                .OrderByDescending(t => t.Likes.Count + t.Comments.Count)
                .Take(5)
                .ToList();

            var tags = _context.Tags
                .GroupBy(t => t.Name)
                .Select(g => new TagCloudViewModel
                {
                    Tag = g.Key,
                    Count = g.Count()
                })
                .ToList();

            var viewModel = new MainPageViewModel
            {
                LatestTemplates = latestTemplates,
                PopularTemplates = popularTemplates,
                TagCloud = tags
            };

            return View(viewModel);
        }
    }
}
