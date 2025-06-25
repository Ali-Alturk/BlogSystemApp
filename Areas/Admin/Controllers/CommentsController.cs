using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BlogSystemApp.Data;
using BlogSystemApp.Models;

namespace BlogSystemApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CommentsController : Controller
    {
        private readonly BlogContext _context;

        public CommentsController(BlogContext context)
        {
            _context = context;
        }

        // GET: Admin/Comments
        public async Task<IActionResult> Index(string statusFilter, int page = 1, int pageSize = 20)
        {
            ViewData["StatusFilter"] = statusFilter;

            var comments = from c in _context.Comments
                          .Include(c => c.Author)
                          .Include(c => c.Post)
                          select c;

            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter == "Approved")
                    comments = comments.Where(c => c.IsApproved);
                else if (statusFilter == "Pending")
                    comments = comments.Where(c => !c.IsApproved);
            }

            var totalItems = await comments.CountAsync();
            var commentList = await comments
                .OrderByDescending(c => c.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(commentList);
        }

        // GET: Admin/Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Admin/Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Admin/Comments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Content,IsApproved,CreatedDate,AuthorId,PostId")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingComment = await _context.Comments.FindAsync(id);
                    if (existingComment == null)
                    {
                        return NotFound();
                    }

                    existingComment.Content = comment.Content;
                    existingComment.IsApproved = comment.IsApproved;

                    _context.Update(existingComment);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Comment updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
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

            // Reload navigation properties for display
            comment = await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(c => c.Id == id);

            return View(comment);
        }

        // GET: Admin/Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Admin/Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Comment deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Comments/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                comment.IsApproved = true;
                _context.Update(comment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Comment approved successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Comments/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                comment.IsApproved = false;
                _context.Update(comment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Comment rejected successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
