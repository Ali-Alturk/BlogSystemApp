using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BlogSystemApp.Data;
using BlogSystemApp.Models;
using BlogSystemApp.Services;

namespace BlogSystemApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly BlogContext _context;
        private readonly IPasswordHashingService _passwordHashingService;

        public UsersController(BlogContext context, IPasswordHashingService passwordHashingService)
        {
            _context = context;
            _passwordHashingService = passwordHashingService;
        }

        // GET: Admin/Users
        public async Task<IActionResult> Index(string searchString, string roleFilter, int page = 1, int pageSize = 10)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["RoleFilter"] = roleFilter;

            var users = from u in _context.Users select u;

            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.Username.Contains(searchString) ||
                                        u.Email.Contains(searchString) ||
                                        u.FirstName.Contains(searchString) ||
                                        u.LastName.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(roleFilter))
            {
                users = users.Where(u => u.Role == roleFilter);
            }

            var totalItems = await users.CountAsync();
            var userList = await users
                .OrderBy(u => u.Username)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(userList);
        }

        // GET: Admin/Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Posts)
                .Include(u => u.Comments)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Admin/Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,Email,Role,FirstName,LastName,Bio,IsActive")] User user, string password)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(password))
                {
                    ModelState.AddModelError("password", "Password is required.");
                    return View(user);
                }

                // Check if username or email already exists
                if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                {
                    ModelState.AddModelError("Username", "Username already exists.");
                    return View(user);
                }

                if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                    return View(user);
                }

                user.PasswordHash = _passwordHashingService.HashPassword(password);
                user.CreatedDate = DateTime.Now;

                _context.Add(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = "User created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Admin/Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Admin/Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Email,Role,FirstName,LastName,Bio,IsActive,CreatedDate,LastLoginDate")] User user, string newPassword)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _context.Users.FindAsync(id);
                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    // Check if username or email already exists (excluding current user)
                    if (await _context.Users.AnyAsync(u => u.Username == user.Username && u.Id != id))
                    {
                        ModelState.AddModelError("Username", "Username already exists.");
                        return View(user);
                    }

                    if (await _context.Users.AnyAsync(u => u.Email == user.Email && u.Id != id))
                    {
                        ModelState.AddModelError("Email", "Email already exists.");
                        return View(user);
                    }

                    existingUser.Username = user.Username;
                    existingUser.Email = user.Email;
                    existingUser.Role = user.Role;
                    existingUser.FirstName = user.FirstName;
                    existingUser.LastName = user.LastName;
                    existingUser.Bio = user.Bio;
                    existingUser.IsActive = user.IsActive;

                    if (!string.IsNullOrEmpty(newPassword))
                    {
                        existingUser.PasswordHash = _passwordHashingService.HashPassword(newPassword);
                    }

                    _context.Update(existingUser);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "User updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }

        // GET: Admin/Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = "User deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
