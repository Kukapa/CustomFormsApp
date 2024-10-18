using Microsoft.AspNetCore.Mvc;
using CustomFormsApp.Models;
using Microsoft.AspNetCore.Authorization;
using CustomFormsApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Diagnostics;

namespace CustomFormsApp.Controllers
{
    [Authorize]
    public class TemplateController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TemplateController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TemplateModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var currentUserId = _userManager.GetUserId(User);

            model.OwnerUserId = currentUserId;

            ModelState.Remove("OwnerUserId");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _context.Templates.AddAsync(model);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating the template.");
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public IActionResult Index()
        {
            var templates = _context.Templates.Include(t => t.Questions).ToList();

            return View(templates);
        }

        private bool TemplateModelExists(int id)
        {
            return _context.Templates.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var template = _context.Templates.Include(t => t.Questions)
                                             .FirstOrDefault(t => t.Id == id);
            if (template == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            if (template.OwnerUserId != currentUserId && !User.IsInRole("Admin")) return Forbid();

            return View(template);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TemplateModel templateModel)
        {
            if (id != templateModel.Id)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(templateModel.OwnerUserId))
            {
                templateModel.OwnerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            ModelState.Remove("OwnerUserId");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(templateModel);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    if (!TemplateModelExists(templateModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(templateModel);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var template = await _context.Templates.FindAsync(id);
            if (template == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            if (template.OwnerUserId != currentUserId && !User.IsInRole("Admin")) return Forbid();

            _context.Templates.Remove(template);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var template = await _context.Templates
                .Include(t => t.Questions)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (template == null)
            {
                return NotFound();
            }

            var formResults = await _context.Answers
                .Where(a => a.Question.TemplateId == id)
                .Include(a => a.Question)
                .Include(a => a.User)
                .ToListAsync();

            var viewModel = new TemplateDetailsViewModel
            {
                Template = template,
                FormResults = formResults
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult AddQuestion(int templateId)
        {
            var model = new AddQuestionViewModel
            {
                TemplateId = templateId
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddQuestion(AddQuestionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var template = await _context.Templates.Include(t => t.Questions).FirstOrDefaultAsync(t => t.Id == model.TemplateId);

                if (template == null)
                {
                    return NotFound();
                }

                var currentUserId = _userManager.GetUserId(User);
                if (template.OwnerUserId != currentUserId && !User.IsInRole("Admin")) 
                    return Forbid();

                var question = new QuestionModel
                {
                    Title = model.Title,
                    Description = model.Description,
                    Type = model.Type,
                    ShowInTable = model.ShowInTable,
                    TemplateId = template.Id
                };

                template.Questions.Add(question);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", new { id = model.TemplateId });
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditQuestion(int questionId)
        {
            var question = await _context.Questions.FindAsync(questionId);

            if (question == null)
            {
                return NotFound();
            }

            var model = new EditQuestionViewModel
            {
                Id = question.Id,
                TemplateId = question.TemplateId,
                Title = question.Title,
                Description = question.Description,
                Type = question.Type,
                ShowInTable = question.ShowInTable
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditQuestion(EditQuestionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var question = await _context.Questions.FindAsync(model.Id);

                if (question == null)
                {
                    return NotFound();
                }

                question.Title = model.Title;
                question.Description = model.Description;
                question.Type = model.Type;
                question.ShowInTable = model.ShowInTable;

                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Template", new { id = model.TemplateId });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Template", new { id = question.TemplateId });
        }
    }
}
