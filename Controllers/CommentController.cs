using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogSystemApp.Data;
using BlogSystemApp.Models;
using BlogSystemApp.Models.ViewModels;

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
                    AnonymousName = model.AnonymousName ?? "Anonymous",
                    AnonymousEmail = model.AnonymousEmail ?? "",
                    AnonymousWebsite = model.AnonymousWebsite ?? "",
                    IsApproved = true, // For demo purposes, auto-approve comments
                    UserAgent = HttpContext.Request.Headers["User-Agent"].ToString(),
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? ""
                };

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Your comment has been posted successfully!";
                return RedirectToAction("Details", "Blog", new { id = model.PostId });
            }

            // If we got this far, something failed, redisplay form
            TempData["Error"] = "There was an error posting your comment. Please try again.";
            return RedirectToAction("Details", "Blog", new { id = model.PostId });
        }
    }
}
