using Microsoft.AspNetCore.Mvc;
using CustomFormsApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using CustomFormsApp.Models;

namespace CustomFormsApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            var userRoles = new List<UserRoleViewModel>();

            foreach (var user in users)
            {
                var roles = _userManager.GetRolesAsync(user).Result;
                userRoles.Add(new UserRoleViewModel
                {
                    User = user,
                    Roles = roles
                });
            }

            return View(userRoles);
        }

        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return View(user);
        }

        public async Task<IActionResult> Block(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.LockoutEnabled = true;
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Unblock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.LockoutEnabled = false; // Disable locking out the user
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddToAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> RemoveFromAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
            }
            return RedirectToAction("Index");
        }
    }
}
