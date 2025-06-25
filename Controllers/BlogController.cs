using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogSystemApp.Data;
using BlogSystemApp.Models;

namespace BlogSystemApp.Controllers
{
    public class BlogController : Controller
    {
        private readonly BlogContext _context;

        public BlogController(BlogContext context)
        {
            _context = context;
        }

        // GET: Blog
        public async Task<IActionResult> Index(string searchString, string sortOrder)
        {
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["CurrentFilter"] = searchString;

            var blogPosts = from b in _context.BlogPosts
                            where b.IsPublished
                            select b;

            if (!String.IsNullOrEmpty(searchString))
            {
                blogPosts = blogPosts.Where(s => s.Title.Contains(searchString)
                                       || s.Content.Contains(searchString)
                                       || s.Tags.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "title_desc":
                    blogPosts = blogPosts.OrderByDescending(s => s.Title);
                    break;
                case "Date":
                    blogPosts = blogPosts.OrderBy(s => s.CreatedDate);
                    break;
                case "date_desc":
                    blogPosts = blogPosts.OrderByDescending(s => s.CreatedDate);
                    break;
                default:
                    blogPosts = blogPosts.OrderByDescending(s => s.CreatedDate);
                    break;
            }

            return View(await blogPosts.AsNoTracking().ToListAsync());
        }

        // GET: Blog/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .FirstOrDefaultAsync(m => m.Id == id && m.IsPublished);
            
            if (blogPost == null)
            {
                return NotFound();
            }

            // Increment view count
            blogPost.ViewCount++;
            _context.Update(blogPost);
            await _context.SaveChangesAsync();

            return View(blogPost);
        }

        // GET: Blog/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Blog/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,Summary,Author,Tags,IsPublished")] BlogPost blogPost)
        {
            if (ModelState.IsValid)
            {
                blogPost.CreatedDate = DateTime.Now;
                _context.Add(blogPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(blogPost);
        }

        // GET: Blog/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }
            return View(blogPost);
        }

        // POST: Blog/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,Summary,Author,Tags,IsPublished,CreatedDate,ViewCount")] BlogPost blogPost)
        {
            if (id != blogPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    blogPost.UpdatedDate = DateTime.Now;
                    _context.Update(blogPost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostExists(blogPost.Id))
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
            return View(blogPost);
        }

        // GET: Blog/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // POST: Blog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost != null)
            {
                _context.BlogPosts.Remove(blogPost);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostExists(int id)
        {
            return _context.BlogPosts.Any(e => e.Id == id);
        }
    }
}
