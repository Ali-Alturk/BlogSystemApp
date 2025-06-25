using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using BlogSystemApp.Data;
using BlogSystemApp.Models;

namespace BlogSystemApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PostsController : Controller
    {
        private readonly BlogContext _context;

        public PostsController(BlogContext context)
        {
            _context = context;
        }

        // GET: Admin/Posts
        public async Task<IActionResult> Index(string searchString, string statusFilter, int? categoryId, int page = 1, int pageSize = 10)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["StatusFilter"] = statusFilter;
            ViewData["CategoryFilter"] = categoryId;

            var posts = from p in _context.Posts
                        .Include(p => p.Author)
                        .Include(p => p.Category)
                        select p;

            if (!string.IsNullOrEmpty(searchString))
            {
                posts = posts.Where(p => p.Title.Contains(searchString) ||
                                        p.Content.Contains(searchString) ||
                                        p.Tags.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter == "Published")
                    posts = posts.Where(p => p.IsPublished);
                else if (statusFilter == "Draft")
                    posts = posts.Where(p => !p.IsPublished);
            }

            if (categoryId.HasValue)
            {
                posts = posts.Where(p => p.CategoryId == categoryId);
            }

            var totalItems = await posts.CountAsync();
            var postList = await posts
                .OrderByDescending(p => p.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");

            return View(postList);
        }

        // GET: Admin/Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Admin/Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Username", post.AuthorId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            return View(post);
        }

        // POST: Admin/Posts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,Summary,Tags,IsPublished,AuthorId,CategoryId,CreatedDate,PublicationDate")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPost = await _context.Posts.FindAsync(id);
                    if (existingPost == null)
                    {
                        return NotFound();
                    }

                    existingPost.Title = post.Title;
                    existingPost.Content = post.Content;
                    existingPost.Summary = post.Summary;
                    existingPost.Tags = post.Tags;
                    existingPost.IsPublished = post.IsPublished;
                    existingPost.AuthorId = post.AuthorId;
                    existingPost.CategoryId = post.CategoryId;
                    existingPost.UpdatedDate = DateTime.Now;

                    if (post.IsPublished && existingPost.PublicationDate == DateTime.MinValue)
                    {
                        existingPost.PublicationDate = DateTime.Now;
                    }

                    _context.Update(existingPost);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Post updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
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

            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Username", post.AuthorId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            return View(post);
        }

        // GET: Admin/Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Admin/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Post deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Posts/TogglePublish/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePublish(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                post.IsPublished = !post.IsPublished;
                post.UpdatedDate = DateTime.Now;

                if (post.IsPublished && post.PublicationDate == DateTime.MinValue)
                {
                    post.PublicationDate = DateTime.Now;
                }

                _context.Update(post);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Post {(post.IsPublished ? "published" : "unpublished")} successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
