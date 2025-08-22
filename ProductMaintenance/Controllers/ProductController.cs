using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using ProductMaintenance.Business.Interfaces;
using ProductMaintenance.Models;

namespace ProductMaintenance.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ProductController : Controller
    {
        private readonly IProductProcess _process;
        private readonly IWebHostEnvironment _env;

        public ProductController(IProductProcess process, IWebHostEnvironment env)
        {
            _process = process;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? q, string? sortField, string? sortDir, int page = 1, int pageSize = 10)
        {
            var result = await _process.SearchAsync(q, page, pageSize, sortField, sortDir);
            return View(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create()
        {
            var model = await _process.GetForCreateAsync();
            return View("Upsert", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create(ProductUpsertModel model, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                return View("Upsert", model);
            }
            string? imageUrl = null;
            if (imageFile != null && imageFile.Length > 0)
            {
                imageUrl = await SaveImageAsync(imageFile);
            }
            var (ok, error) = await _process.CreateAsync(model, imageUrl);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error ?? "Failed to create product");
                return View("Upsert", model);
            }
            TempData["Success"] = "Product created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _process.GetForEditAsync(id);
            if (model == null) return NotFound();
            return View("Upsert", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductUpsertModel model, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                return View("Upsert", model);
            }
            string? imageUrl = null;
            if (imageFile != null && imageFile.Length > 0)
            {
                imageUrl = await SaveImageAsync(imageFile);
            }
            var (ok, error) = await _process.UpdateAsync(model, imageUrl);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error ?? "Failed to update product");
                return View("Upsert", model);
            }
            TempData["Success"] = "Product updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var (ok, error) = await _process.DeleteAsync(id);
            if (!ok)
            {
                TempData["Error"] = error ?? "Failed to delete product";
            }
            else
            {
                TempData["Success"] = "Product deleted";
            }
            return RedirectToAction(nameof(Index));
        }

        private static List<string> GetCategories() => new() { "Electronics", "Clothing", "Groceries", "Books", "Furniture", "Sports" };

        private async Task<string> SaveImageAsync(IFormFile file)
        {
            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "products");
            if (!Directory.Exists(uploadsDir)) Directory.CreateDirectory(uploadsDir);

            var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(uploadsDir, fileName);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return $"/uploads/products/{fileName}";
        }
    }
}
