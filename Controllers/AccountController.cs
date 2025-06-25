using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BlogSystemApp.Data;
using BlogSystemApp.Models;
using BlogSystemApp.Models.ViewModels;
using BlogSystemApp.Services;

namespace BlogSystemApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly BlogContext _context;
        private readonly IPasswordHashingService _passwordHashingService;

        public AccountController(BlogContext context, IPasswordHashingService passwordHashingService)
        {
            _context = context;
            _passwordHashingService = passwordHashingService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == model.Username && u.IsActive);

                if (user != null && _passwordHashingService.VerifyPassword(model.Password, user.PasswordHash))
                {
                    // Update last login date
                    user.LastLoginDate = DateTime.Now;
                    await _context.SaveChangesAsync();

                    // Create claims
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role),
                        new Claim("DisplayName", user.DisplayName)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(1)
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), authProperties);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid username or password.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if username already exists
                if (await _context.Users.AnyAsync(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "Username already exists.");
                    return View(model);
                }

                // Check if email already exists
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email address already exists.");
                    return View(model);
                }

                // Create new user
                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = _passwordHashingService.HashPassword(model.Password),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Bio = model.Bio,
                    Role = "User", // Default role
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Registration successful! Please log in.";
                return RedirectToAction("Login");
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users
                .Include(u => u.Posts)
                .Include(u => u.Comments)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            // Calculate statistics
            var totalPosts = user.Posts.Count;
            var publishedPosts = user.Posts.Count(p => p.IsPublished);
            var totalComments = user.Comments.Count;
            var totalViews = user.Posts.Sum(p => p.ViewCount);

            var model = new UserProfileViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Bio = user.Bio,
                ProfilePictureUrl = user.ProfilePicture,
                Role = Enum.Parse<UserRole>(user.Role),
                IsActive = user.IsActive,
                CreatedDate = user.CreatedDate,
                LastLoginDate = user.LastLoginDate,
                TotalPosts = totalPosts,
                PublishedPosts = publishedPosts,
                TotalComments = totalComments,
                TotalViews = totalViews
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UserProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    return NotFound();
                }

                // Update user profile
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Bio = model.Bio ?? string.Empty;
                user.ProfilePicture = model.ProfilePictureUrl ?? string.Empty;

                await _context.SaveChangesAsync();

                TempData["Message"] = "Profile updated successfully!";
                return RedirectToAction(nameof(Profile));
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    return NotFound();
                }

                if (!_passwordHashingService.VerifyPassword(model.CurrentPassword, user.PasswordHash))
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                    return View(model);
                }

                user.PasswordHash = _passwordHashingService.HashPassword(model.NewPassword);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Password changed successfully!";
                return RedirectToAction("Profile");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // Admin area for user management
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageUsers()
        {
            var users = await _context.Users
                .OrderBy(u => u.Username)
                .ToListAsync();

            return View(users);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"User {user.Username} has been {(user.IsActive ? "activated" : "deactivated")}.";
            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUserRole(int id, string role)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var validRoles = new[] { "User", "Editor", "Admin" };
            if (!validRoles.Contains(role))
            {
                TempData["ErrorMessage"] = "Invalid role specified.";
                return RedirectToAction("ManageUsers");
            }

            user.Role = role;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"User {user.Username} role changed to {role}.";
            return RedirectToAction("ManageUsers");
        }
    }
}
