using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BlogSystemApp.Data;
using BlogSystemApp.Models;
using BlogSystemApp.Models.ViewModels;

namespace BlogSystemApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly BlogContext _context;

        public CategoriesController(BlogContext context)
        {
            _context = context;
        }

        // GET: Admin/Categories
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Include(c => c.Posts)
                .ToListAsync();
            return View(categories);
        }

        // GET: Admin/Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.Posts)
                    .ThenInclude(p => p.Author)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            var postCount = category.Posts.Count;
            var publishedPostCount = category.Posts.Count(p => p.IsPublished);
            var recentPosts = category.Posts
                .OrderByDescending(p => p.CreatedDate)
                .Take(5);

            var viewModel = new CategoryManagementViewModel
            {
                Category = category,
                PostCount = postCount,
                PublishedPostCount = publishedPostCount,
                RecentPosts = recentPosts,
                CanDelete = postCount == 0 // Can only delete if no posts
            };

            return View(viewModel);
        }

        // GET: Admin/Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Color")] Category category)
        {
            if (ModelState.IsValid)
            {
                // Check if category name already exists
                if (await _context.Categories.AnyAsync(c => c.Name == category.Name))
                {
                    ModelState.AddModelError("Name", "Category name already exists.");
                    return View(category);
                }

                category.CreatedDate = DateTime.Now;
                _context.Add(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Category created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Admin/Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Admin/Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Color,CreatedDate")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if category name already exists (excluding current category)
                    if (await _context.Categories.AnyAsync(c => c.Name == category.Name && c.Id != id))
                    {
                        ModelState.AddModelError("Name", "Category name already exists.");
                        return View(category);
                    }

                    _context.Update(category);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Category updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Admin/Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.Posts)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Posts)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category != null)
            {
                // Check if category has posts
                if (category.Posts.Any())
                {
                    TempData["Error"] = "Cannot delete category that contains posts. Please move or delete the posts first.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Category deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
