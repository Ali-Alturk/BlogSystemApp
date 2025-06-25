using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BlogSystemApp.Data;
using BlogSystemApp.Models;
using BlogSystemApp.Models.ViewModels;
using System.Security.Claims;

namespace BlogSystemApp.Controllers
{
    public class CommentController : Controller
    {
        private readonly BlogContext _context;

        public CommentController(BlogContext context)
        {
            _context = context;
        }

        // POST: Comment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CommentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var comment = new Comment
                {
                    Content = model.Content,
                    PostId = model.PostId,
                    CommentDate = DateTime.Now,
                    CreatedDate = DateTime.Now,
                    IsApproved = true, // For demo purposes, auto-approve comments
                    UserAgent = HttpContext.Request.Headers["User-Agent"].ToString(),
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? ""
                };

                // Set author if user is authenticated
                if (User.Identity!.IsAuthenticated)
                {
                    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                    comment.AuthorId = userId;
                }
                else
                {
                    // For anonymous comments
                    comment.AnonymousName = model.AnonymousName ?? "Anonymous";
                    comment.AnonymousEmail = model.AnonymousEmail ?? "";
                    comment.AnonymousWebsite = model.AnonymousWebsite ?? "";
                }

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Your comment has been posted successfully!";
                return RedirectToAction("Details", "Blog", new { id = model.PostId });
            }

            // If we got this far, something failed, redisplay form
            TempData["Message"] = "There was an error posting your comment. Please try again.";
            return RedirectToAction("Details", "Blog", new { id = model.PostId });
        }

        // GET: Comment/Edit/5
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var comment = await _context.Comments
                .Include(c => c.Post)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null)
            {
                return NotFound();
            }

            // Check if user can edit this comment
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (comment.AuthorId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var model = new CommentCreateViewModel
            {
                Content = comment.Content,
                PostId = comment.PostId,
                PostTitle = comment.Post?.Title ?? ""
            };

            ViewBag.CommentId = id;
            return View(model);
        }

        // POST: Comment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, CommentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var comment = await _context.Comments.FindAsync(id);
                if (comment == null)
                {
                    return NotFound();
                }

                // Check if user can edit this comment
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                if (comment.AuthorId != userId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                comment.Content = model.Content;
                comment.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                TempData["Message"] = "Comment updated successfully!";
                return RedirectToAction("Details", "Blog", new { id = comment.PostId });
            }

            return View(model);
        }

        // POST: Comment/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            // Check if user can delete this comment
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (comment.AuthorId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var postId = comment.PostId;
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Comment deleted successfully!";
            return RedirectToAction("Details", "Blog", new { id = postId });
        }

        // GET: Comment/MyComments
        [Authorize]
        public async Task<IActionResult> MyComments(int page = 1, int pageSize = 10)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            var comments = _context.Comments
                .Include(c => c.Post)
                .Where(c => c.AuthorId == userId)
                .OrderByDescending(c => c.CreatedDate);

            var totalItems = await comments.CountAsync();
            var commentList = await comments
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(commentList);
        }
    }
}
