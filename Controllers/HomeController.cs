using BlogSystemApp.Models;
using BlogSystemApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BlogSystemApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BlogContext _context;

        public HomeController(ILogger<HomeController> logger, BlogContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Public blog listing page
        public async Task<IActionResult> Index(string searchString, int? categoryId, int page = 1, int pageSize = 6)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentCategory"] = categoryId;

            var posts = from p in _context.Posts
                        .Include(p => p.Author)
                        .Include(p => p.Category)
                        where p.IsPublished
                        select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                posts = posts.Where(s => s.Title.Contains(searchString)
                                   || s.Content.Contains(searchString)
                                   || s.Tags.Contains(searchString));
            }

            if (categoryId.HasValue)
            {
                posts = posts.Where(p => p.CategoryId == categoryId);
            }

            // Get featured posts for carousel
            var featuredPosts = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Where(p => p.IsPublished && p.IsFeatured)
                .OrderByDescending(p => p.PublicationDate)
                .Take(3)
                .ToListAsync();

            var totalItems = await posts.CountAsync();
            var postList = await posts
                .OrderByDescending(p => p.PublicationDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var categories = await _context.Categories.ToListAsync();

            ViewBag.FeaturedPosts = featuredPosts;
            ViewBag.Categories = categories;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(postList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
