﻿using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Index()
        {
            var templates = await _context.Templates
                .Include(t => t.Questions)
                .ToListAsync();

            foreach (var template in templates)
            {
                var owner = await _userManager.FindByIdAsync(template.OwnerUserId);
                template.OwnerEmail = owner?.Email;
            }

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
    }
}
