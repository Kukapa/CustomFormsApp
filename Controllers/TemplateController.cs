using Microsoft.AspNetCore.Mvc;
using CustomFormsApp.Models;
using Microsoft.AspNetCore.Authorization;
using CustomFormsApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
            var model = new TemplateModel
            {
                Tags = new List<TagModel>()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TemplateModel model, string tagNames)
        {
            model.CreatedDate = DateTime.UtcNow;

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var currentUserId = _userManager.GetUserId(User);
            model.OwnerUserId = currentUserId;

            ModelState.Remove("OwnerUserId");

            if (!ModelState.IsValid)
            {
                model.Tags = model.Tags ?? new List<TagModel>();
                return View(model);
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(tagNames))
                {
                    var tagList = tagNames.Split(',')
                        .Select(t => new TagModel { Name = t.Trim() })
                        .ToList();
                    model.Tags.AddRange(tagList);
                }

                await _context.Templates.AddAsync(model);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                ModelState.AddModelError(string.Empty, "An error occurred while creating the template.");
                return View(model);
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var templates = await _context.Templates
                .Include(t => t.Questions)
                .Include(t => t.Tags)
                .ToListAsync();

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
            var template = _context.Templates
                .Include(t => t.Questions)
                .Include(t => t.Tags)
                .FirstOrDefault(t => t.Id == id);

            if (template == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            if (template.OwnerUserId != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return View(template);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TemplateModel templateModel, string[] tagNames)
        {
            if (id != templateModel.Id)
            {
                return NotFound();
            }

            var existingTemplate = await _context.Templates
                .Include(t => t.Tags)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (existingTemplate == null)
            {
                return NotFound();
            }

            existingTemplate.Title = templateModel.Title;
            existingTemplate.Description = templateModel.Description;
            existingTemplate.IsPublic = templateModel.IsPublic;
            existingTemplate.Topic = templateModel.Topic;
            existingTemplate.Tags = templateModel.Tags;

            if (tagNames != null && tagNames.Length > 0)
            {
                foreach (var tagName in tagNames)
                {
                    var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                    if (existingTag != null)
                    {
                        existingTemplate.Tags.Add(existingTag);
                    }
                    else
                    {
                        var newTag = new TagModel { Name = tagName };
                        existingTemplate.Tags.Add(newTag);
                        _context.Tags.Add(newTag); 
                    }
                }
            }

            ModelState.Remove("OwnerUserId");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(existingTemplate);
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
                .Include(t => t.FilledForms)
                .ThenInclude(ff => ff.User)
                .Include(t => t.Comments)
                .Include(t => t.Likes)
                .Include(t => t.Tags)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (template == null)
            {
                return NotFound();
            }

            var numericQuestions = template.Questions.Where(q => q.Type == QuestionType.PositiveInteger);
            var averages = new Dictionary<int, double>();

            foreach (var question in numericQuestions)
            {
                var answers = template.FilledForms
                    .SelectMany(f => f.Answers)
                    .Where(a => a.QuestionId == question.Id && a.AnswerInteger.HasValue)
                    .Select(a => a.AnswerInteger.Value)
                    .ToList();

                averages[question.Id] = answers.Any() ? answers.Average() : 0;
            }          

            var user = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Admin");
            var canManageTemplate = isAdmin || template.OwnerUserId == user.Id;
            
            bool hasUserLiked = template.Likes.Any(l => l.UserName == user.Id);
            
            var viewModel = new TemplateDetailsViewModel
            {
                Template = template,
                FormResults = template.FilledForms.ToList(),
                CanManageTemplate = canManageTemplate,
                IsAdmin = isAdmin,
                Averages = averages,
                HasUserLiked = hasUserLiked,
                Tags = template.Tags
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

        [HttpGet]
        public async Task<IActionResult> ViewAnswers(int filledFormId)
        {
            var filledForm = await _context.FilledForms
                .Include(ff => ff.Answers)
                .ThenInclude(a => a.Question)
                .Include(ff => ff.User)
                .FirstOrDefaultAsync(ff => ff.Id == filledFormId);

            if (filledForm == null)
            {
                return NotFound();
            }

            var viewModel = new ViewAnswersViewModel
            {
                FilledForm = filledForm,
                UserName = filledForm.User.UserName
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleLike(int templateId)
        {
            var user = await _userManager.GetUserAsync(User);
            var template = await _context.Templates
                .Include(t => t.Likes)
                .FirstOrDefaultAsync(t => t.Id == templateId);

            if (template == null || user == null)
            {
                return NotFound();
            }

            var existingLike = template.Likes.FirstOrDefault(l => l.UserName == user.UserName);
            if (existingLike != null)
            {
                _context.Likes.Remove(existingLike);
            }
            else
            {
                template.Likes.Add(new LikeModel
                {
                    TemplateId = templateId,
                    UserName = user.UserName
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = templateId });
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int templateId, string commentContent)
        {
            if (string.IsNullOrWhiteSpace(commentContent) || templateId <= 0)
            {
                return BadRequest("Invalid data.");
            }

            var template = await _context.Templates
                                         .Include(t => t.Comments)
                                         .FirstOrDefaultAsync(t => t.Id == templateId);

            if (template == null)
            {
                return NotFound("Template not found.");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("You must be logged in to post a comment.");
            }

            var comment = new CommentModel
            {
                Content = commentContent,
                CreatedDate = DateTime.UtcNow,
                UserName = user.UserName,
                TemplateId = templateId
            };

            template.Comments.Add(comment);
            _context.Update(template);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = templateId });
        }
    }
}
