using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogSystemApp.Data;
using BlogSystemApp.Models;

namespace BlogSystemApp.ViewComponents
{
    public class RecentPostsViewComponent : ViewComponent
    {
        private readonly BlogContext _context;

        public RecentPostsViewComponent(BlogContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int count = 5)
        {
            var recentPosts = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedDate)
                .Take(count)
                .Select(p => new RecentPostViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    CreatedAt = p.CreatedDate,
                    AuthorName = p.Author.Username,
                    CategoryName = p.Category != null ? p.Category.Name : "Uncategorized",
                    CommentCount = p.Comments.Count()
                })
                .ToListAsync();

            return View(recentPosts);
        }
    }

    public class RecentPostViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int CommentCount { get; set; }
    }
}
