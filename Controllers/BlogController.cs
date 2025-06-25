using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogSystemApp.Data;
using BlogSystemApp.Models;
using BlogSystemApp.Models.ViewModels;

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
        public async Task<IActionResult> Index(string searchString, string sortOrder, int? categoryId, int page = 1, int pageSize = 10)
        {
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
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

            switch (sortOrder)
            {
                case "title_desc":
                    posts = posts.OrderByDescending(s => s.Title);
                    break;
                case "Date":
                    posts = posts.OrderBy(s => s.PublicationDate);
                    break;
                case "date_desc":
                    posts = posts.OrderByDescending(s => s.PublicationDate);
                    break;
                default:
                    posts = posts.OrderByDescending(s => s.PublicationDate);
                    break;
            }

            var totalPosts = await posts.CountAsync();
            var totalPages = (int)Math.Ceiling(totalPosts / (double)pageSize);

            var paginatedPosts = await posts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .AsNoTracking()
                .ToListAsync();

            var featuredPosts = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Where(p => p.IsPublished && p.IsFeatured)
                .OrderByDescending(p => p.PublicationDate)
                .Take(3)
                .AsNoTracking()
                .ToListAsync();

            var viewModel = new BlogIndexViewModel
            {
                Posts = paginatedPosts,
                Categories = categories,
                FeaturedPosts = featuredPosts,
                SearchTerm = searchString,
                SortOrder = sortOrder,
                CategoryId = categoryId,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalPosts = totalPosts
            };

            return View(viewModel);
        }

        // GET: Blog/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id && m.IsPublished);
            
            if (post == null)
            {
                return NotFound();
            }

            // Increment view count
            post.ViewCount++;
            _context.Update(post);
            await _context.SaveChangesAsync();

            // Get comments
            var comments = await _context.Comments
                .Include(c => c.Author)
                .Where(c => c.PostId == id && c.IsApproved && !c.IsDeleted)
                .OrderBy(c => c.CommentDate)
                .AsNoTracking()
                .ToListAsync();

            // Get related posts
            var relatedPosts = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Where(p => p.Id != id && p.IsPublished && 
                           (p.CategoryId == post.CategoryId))
                .OrderByDescending(p => p.PublicationDate)
                .Take(3)
                .AsNoTracking()
                .ToListAsync();

            var viewModel = new PostDetailsViewModel
            {
                Post = post,
                Comments = comments,
                RelatedPosts = relatedPosts,
                NewComment = new CommentCreateViewModel { PostId = post.Id }
            };

            return View(viewModel);
        }

        // GET: Blog/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsActive)
                .ToListAsync();
            return View();
        }

        // POST: Blog/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,Summary,Tags,IsFeatured,CategoryId")] Post post)
        {
            if (ModelState.IsValid)
            {
                // For now, use a default author (in a real app, you'd get this from authentication)
                post.AuthorId = 1; // Default to admin user
                post.CreatedDate = DateTime.Now;
                post.PublicationDate = DateTime.Now;
                post.IsPublished = true;
                post.GenerateSlug();
                
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsActive)
                .ToListAsync();
            return View(post);
        }

        // GET: Blog/Edit/5
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
            
            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsActive)
                .ToListAsync();
            return View(post);
        }

        // POST: Blog/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,Summary,Tags,IsFeatured,CategoryId,PublicationDate,CreatedDate,ViewCount,AuthorId,IsPublished")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    post.UpdatedDate = DateTime.Now;
                    post.GenerateSlug();
                    _context.Update(post);
                    await _context.SaveChangesAsync();
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
            
            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsActive)
                .ToListAsync();
            return View(post);
        }

        // GET: Blog/Delete/5
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

        // POST: Blog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
