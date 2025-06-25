using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BlogSystemApp.Data;
using BlogSystemApp.Models;
using BlogSystemApp.Models.ViewModels;
using System.Security.Claims;

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
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

            var viewModel = new PostCreateEditViewModel
            {
                Categories = categories,
                IsPublished = true,
                PublicationDate = DateTime.Now
            };

            return View(viewModel);
        }

        // POST: Blog/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(PostCreateEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Get the current user ID from claims
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                
                var post = new Post
                {
                    Title = model.Title,
                    Content = model.Content,
                    Summary = model.Summary ?? string.Empty,
                    Tags = model.Tags ?? string.Empty,
                    IsFeatured = model.IsFeatured,
                    CategoryId = model.CategoryId,
                    FeaturedImage = model.FeaturedImage ?? string.Empty,
                    IsPublished = model.IsPublished,
                    AuthorId = userId,
                    CreatedDate = DateTime.Now,
                    PublicationDate = model.IsPublished ? DateTime.Now : DateTime.Now
                };
                
                // Only Admins and Editors can set featured posts
                if (!User.IsInRole("Admin") && !User.IsInRole("Editor"))
                {
                    post.IsFeatured = false;
                }
                
                post.GenerateSlug();
                
                _context.Add(post);
                await _context.SaveChangesAsync();
                
                TempData["Message"] = "Post created successfully!";
                return RedirectToAction(nameof(Details), new { id = post.Id });
            }
            
            // Reload categories if validation failed
            model.Categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
            
            return View(model);
        }

        // GET: Blog/Edit/5
        [Authorize]
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
            
            // Check if user can edit this post
            if (!CanEditPost(post))
            {
                return Forbid();
            }
            
            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsActive)
                .ToListAsync();
            return View(post);
        }

        // POST: Blog/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,Summary,Tags,IsFeatured,CategoryId,PublicationDate,CreatedDate,ViewCount,AuthorId,IsPublished")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            // Get the original post to check permissions
            var originalPost = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (originalPost == null)
            {
                return NotFound();
            }
            
            // Check if user can edit this post
            if (!CanEditPost(originalPost))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Only Admins and Editors can set featured posts
                    if (!User.IsInRole("Admin") && !User.IsInRole("Editor"))
                    {
                        post.IsFeatured = originalPost.IsFeatured; // Keep original value
                    }
                    
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
        [Authorize]
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
            
            // Check if user can delete this post
            if (!CanEditPost(post))
            {
                return Forbid();
            }

            return View(post);
        }

        // POST: Blog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            
            // Check if user can delete this post
            if (!CanEditPost(post))
            {
                return Forbid();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Private area - User's own posts management
        [Authorize]
        public async Task<IActionResult> MyPosts(string searchString, string statusFilter, int page = 1, int pageSize = 10)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            ViewData["CurrentFilter"] = searchString;
            ViewData["StatusFilter"] = statusFilter;

            var posts = from p in _context.Posts
                        .Include(p => p.Category)
                        where p.AuthorId == userId
                        select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                posts = posts.Where(s => s.Title.Contains(searchString)
                                   || s.Content.Contains(searchString)
                                   || s.Tags.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter == "Published")
                    posts = posts.Where(p => p.IsPublished);
                else if (statusFilter == "Draft")
                    posts = posts.Where(p => !p.IsPublished);
            }

            var totalItems = await posts.CountAsync();
            var postList = await posts
                .OrderByDescending(p => p.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(postList);
        }

        // GET: Blog/Search
        public IActionResult Search(string q, int? categoryId, string tag, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(q) && !categoryId.HasValue && string.IsNullOrWhiteSpace(tag))
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index), new { searchString = q, categoryId, page });
        }

        // GET: Blog/ByCategory/5
        public async Task<IActionResult> ByCategory(int id, int page = 1)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index), new { categoryId = id, page });
        }

        // GET: Blog/ByTag/tag-name
        public async Task<IActionResult> ByTag(string tag, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                return NotFound();
            }

            var posts = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Where(p => p.IsPublished && p.Tags.Contains(tag))
                .OrderByDescending(p => p.PublicationDate)
                .Skip((page - 1) * 10)
                .Take(10)
                .ToListAsync();

            ViewBag.Tag = tag;
            ViewBag.CurrentPage = page;
            
            return View("Index", posts);
        }

        // POST: Blog/ToggleFeatured/5
        [HttpPost]
        [Authorize(Roles = "Admin,Editor")]
        public async Task<IActionResult> ToggleFeatured(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            post.IsFeatured = !post.IsFeatured;
            post.UpdatedDate = DateTime.Now;
            
            await _context.SaveChangesAsync();
            
            TempData["Message"] = post.IsFeatured ? "Post marked as featured" : "Post unmarked as featured";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Blog/TogglePublish/5
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> TogglePublish(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            // Check if user can edit this post
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (post.AuthorId != userId && !User.IsInRole("Admin") && !User.IsInRole("Editor"))
            {
                return Forbid();
            }

            post.IsPublished = !post.IsPublished;
            post.PublicationDate = post.IsPublished ? DateTime.Now : post.PublicationDate;
            post.UpdatedDate = DateTime.Now;
            
            await _context.SaveChangesAsync();
            
            TempData["Message"] = post.IsPublished ? "Post published successfully" : "Post moved to drafts";
            return RedirectToAction(nameof(MyPosts));
        }

        // GET: Blog/Preview/5
        [Authorize]
        public async Task<IActionResult> Preview(int id)
        {
            var post = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.Comments.Where(c => c.IsApproved))
                .ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            // Check if user can preview this post
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (post.AuthorId != userId && !User.IsInRole("Admin") && !User.IsInRole("Editor"))
            {
                return Forbid();
            }

            // Get related posts
            var relatedPosts = await _context.Posts
                .Where(p => p.IsPublished && p.Id != post.Id && p.CategoryId == post.CategoryId)
                .Take(3)
                .ToListAsync();

            var viewModel = new PostDetailsViewModel
            {
                Post = post,
                Comments = post.Comments.OrderBy(c => c.CreatedDate),
                RelatedPosts = relatedPosts,
                NewComment = new CommentCreateViewModel { PostId = post.Id, PostTitle = post.Title },
                CanEdit = true,
                CanDelete = true,
                TotalComments = post.Comments.Count,
                ApprovedComments = post.Comments.Count(c => c.IsApproved)
            };

            ViewBag.IsPreview = true;
            return View("Details", viewModel);
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
        
        private bool CanEditPost(Post post)
        {
            // Admins can edit any post
            if (User.IsInRole("Admin"))
            {
                return true;
            }

            // Editors can edit any post
            if (User.IsInRole("Editor"))
            {
                return true;
            }

            // Users can only edit their own posts
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return post.AuthorId == userId;
        }
    }
}
