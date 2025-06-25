using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BlogSystemApp.Data;

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
            var totalComments = await _context.Comments.CountAsync();
            var totalCategories = await _context.Categories.CountAsync();
            
            var recentPosts = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedDate)
                .Take(5)
                .ToListAsync();

            var recentUsers = await _context.Users
                .OrderByDescending(u => u.CreatedDate)
                .Take(5)
                .ToListAsync();

            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalPosts = totalPosts;
            ViewBag.TotalComments = totalComments;
            ViewBag.TotalCategories = totalCategories;
            ViewBag.RecentPosts = recentPosts;
            ViewBag.RecentUsers = recentUsers;

            return View();
        }
    }
}
