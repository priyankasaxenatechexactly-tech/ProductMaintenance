using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ProductMaintenance.Business.Interfaces;
using ProductMaintenance.Models;
using System.Threading.Tasks;

namespace ProductMaintenance.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IUserProcess _process;

        public UserController(IUserProcess process)
        {
            _process = process;
        }

        // GET: /User
        public async Task<IActionResult> Index(string? q, int page = 1, int pageSize = 10)
        {
            int? currentUserId = null;
            var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrWhiteSpace(idStr) && int.TryParse(idStr, out var parsed))
            {
                currentUserId = parsed;
            }
            var result = await _process.SearchAsync(q, page, pageSize, currentUserId);
            return View(result);
            }

        // GET: /User/Create
        public async Task<IActionResult> Create()
        {
            var model = await _process.GetForCreateAsync();
            ViewData["Title"] = "Create User";
            return View("Upsert", model);
        }

        // POST: /User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserUpsertModel model)
        {
            if (!ModelState.IsValid)
            {
                var roles = await _process.GetForCreateAsync();
                model.Roles = roles.Roles;
                ViewData["Title"] = "Create User";
                return View("Upsert", model);
            }
            var (ok, error) = await _process.CreateAsync(model);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error ?? "Error creating user.");
                var roles = await _process.GetForCreateAsync();
                model.Roles = roles.Roles;
                ViewData["Title"] = "Create User";
                return View("Upsert", model);
            }
            TempData["Success"] = "User created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /User/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _process.GetForEditAsync(id);
            if (model == null) return NotFound();
            ViewData["Title"] = "Edit User";
            return View("Upsert", model);
        }

        // POST: /User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserUpsertModel model)
        {
            if (!ModelState.IsValid)
            {
                var reload = await _process.GetForEditAsync(model.Id);
                if (reload != null) model.Roles = reload.Roles;
                ViewData["Title"] = "Edit User";
                return View("Upsert", model);
            }
            var (ok, error) = await _process.UpdateAsync(model);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error ?? "Error updating user.");
                var reload = await _process.GetForEditAsync(model.Id);
                if (reload != null) model.Roles = reload.Roles;
                ViewData["Title"] = "Edit User";
                return View("Upsert", model);
            }
            TempData["Success"] = "User updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /User/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (ok, error) = await _process.DeleteAsync(id);
            if (!ok)
            {
                TempData["Error"] = error ?? "Error deleting user.";
            }
            else
            {
                TempData["Success"] = "User deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
