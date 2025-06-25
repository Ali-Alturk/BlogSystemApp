using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BlogSystemApp.Data;
using BlogSystemApp.Models.ViewModels;

namespace BlogSystemApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly BlogContext _context;

        public HomeController(BlogContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Dashboard statistics
            var totalUsers = await _context.Users.CountAsync();
            var totalPosts = await _context.Posts.CountAsync();
            var publishedPosts = await _context.Posts.CountAsync(p => p.IsPublished);
            var draftPosts = totalPosts - publishedPosts;
            var totalComments = await _context.Comments.CountAsync();
            var pendingComments = await _context.Comments.CountAsync(c => !c.IsApproved);
            var totalCategories = await _context.Categories.CountAsync();
            var totalViews = await _context.Posts.SumAsync(p => p.ViewCount);
            
            var recentPosts = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedDate)
                .Take(5)
                .ToListAsync();

            var recentComments = await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.Post)
                .OrderByDescending(c => c.CreatedDate)
                .Take(5)
                .ToListAsync();

            var recentUsers = await _context.Users
                .OrderByDescending(u => u.CreatedDate)
                .Take(5)
                .ToListAsync();

            // Posts by category for charts
            var postsByCategory = await _context.Posts
                .Include(p => p.Category)
                .GroupBy(p => p.Category!.Name)
                .Select(g => new { Category = g.Key ?? "Uncategorized", Count = g.Count() })
                .ToDictionaryAsync(x => x.Category, x => x.Count);

            // Posts by date (last 7 days)
            var sevenDaysAgo = DateTime.Now.AddDays(-7);
            var postsByDate = await _context.Posts
                .Where(p => p.CreatedDate >= sevenDaysAgo)
                .GroupBy(p => p.CreatedDate.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Date, x => x.Count);

            // Comments by status
            var commentsByStatus = new Dictionary<string, int>
            {
                { "Approved", await _context.Comments.CountAsync(c => c.IsApproved) },
                { "Pending", await _context.Comments.CountAsync(c => !c.IsApproved) }
            };

            var dashboardViewModel = new DashboardViewModel
            {
                TotalUsers = totalUsers,
                TotalPosts = totalPosts,
                PublishedPosts = publishedPosts,
                DraftPosts = draftPosts,
                TotalComments = totalComments,
                PendingComments = pendingComments,
                TotalCategories = totalCategories,
                TotalViews = totalViews,
                RecentPosts = recentPosts,
                RecentComments = recentComments,
                RecentUsers = recentUsers,
                PostsByCategory = postsByCategory,
                PostsByDate = postsByDate,
                CommentsByStatus = commentsByStatus
            };

            return View(dashboardViewModel);
        }
    }
}
