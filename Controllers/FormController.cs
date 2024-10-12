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

        [HttpGet]
        public async Task<IActionResult> Answer(int templateId)
        {
            var questions = await _context.Questions
                .Where(q => q.TemplateId == templateId)
                .ToListAsync();

            return View(questions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitAnswers(List<int> QuestionIds, Dictionary<int, string> Answers)
        {
            var userId = _userManager.GetUserId(User);

            foreach (var questionId in QuestionIds)
            {
                var question = await _context.Questions.FindAsync(questionId);

                if (question == null)
                {
                    continue;
                }

                var answer = new AnswerModel
                {
                    QuestionId = questionId,
                    UserId = userId,
                    SubmittedAt = DateTime.UtcNow
                };

                if (question.QuestionType == "SingleLineString" || question.QuestionType == "MultiLineText")
                {
                    answer.AnswerText = Answers[questionId];
                }
                else if (question.QuestionType == "PositiveInteger" && int.TryParse(Answers[questionId], out int integerAnswer))
                {
                    answer.AnswerInteger = integerAnswer;
                }
                else if (question.QuestionType == "Checkbox")
                {
                    answer.AnswerBoolean = Answers.ContainsKey(questionId) && Answers[questionId] == "true";
                }

                _context.Answers.Add(answer);
            }

            await _context.SaveChangesAsync();

            var templateId = _context.Questions
                .Where(q => QuestionIds.Contains(q.Id))
                .Select(q => q.TemplateId)
                .FirstOrDefault();

            return RedirectToAction("Result", "Form", new { templateId });
        }

        [HttpGet]
        public async Task<IActionResult> Result(int templateId)
        {
            var results = await _context.Answers
                .Where(a => a.Question.TemplateId == templateId)
                .Include(a => a.Question)
                .ToListAsync();

            return View(results);
        }
    }
}
