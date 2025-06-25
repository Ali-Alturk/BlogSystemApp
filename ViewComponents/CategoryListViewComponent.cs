using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogSystemApp.Data;
using BlogSystemApp.Models;

namespace BlogSystemApp.ViewComponents
{
    public class CategoryListViewComponent : ViewComponent
    {
        private readonly BlogContext _context;

        public CategoryListViewComponent(BlogContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool showPostCount = true)
        {
            var categories = await _context.Categories
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    PostCount = showPostCount ? c.Posts.Count(p => p.IsPublished) : 0
                })
                .OrderBy(c => c.Name)
                .ToListAsync();

            ViewBag.ShowPostCount = showPostCount;
            return View(categories);
        }
    }

    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PostCount { get; set; }
    }
}
