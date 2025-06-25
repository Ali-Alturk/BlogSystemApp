using Microsoft.EntityFrameworkCore;
using BlogSystemApp.Models;

namespace BlogSystemApp.Data
{
    public class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions<BlogContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Role).HasMaxLength(20).IsRequired();
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
                
                // Unique constraints
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configure Category entity
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Slug).HasMaxLength(100);
                entity.Property(e => e.Color).HasMaxLength(20);
                
                // Unique constraint for slug
                entity.HasIndex(e => e.Slug).IsUnique();
            });

            // Configure Post entity
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.Summary).HasMaxLength(500);
                entity.Property(e => e.Slug).HasMaxLength(200);
                entity.Property(e => e.Tags).HasMaxLength(500);
                entity.Property(e => e.FeaturedImage).HasMaxLength(500);
                entity.Property(e => e.MetaDescription).HasMaxLength(300);
                
                // Configure relationships
                entity.HasOne(e => e.Author)
                      .WithMany(u => u.Posts)
                      .HasForeignKey(e => e.AuthorId)
                      .OnDelete(DeleteBehavior.Restrict);
                      
                entity.HasOne(e => e.Category)
                      .WithMany(c => c.Posts)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.SetNull);
                      
                // Unique constraint for slug
                entity.HasIndex(e => e.Slug).IsUnique();
            });

            // Configure Comment entity
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.AnonymousName).HasMaxLength(100);
                entity.Property(e => e.AnonymousEmail).HasMaxLength(255);
                entity.Property(e => e.AnonymousWebsite).HasMaxLength(255);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                entity.Property(e => e.IpAddress).HasMaxLength(45);
                
                // Configure relationships
                entity.HasOne(e => e.Post)
                      .WithMany(p => p.Comments)
                      .HasForeignKey(e => e.PostId)
                      .OnDelete(DeleteBehavior.Cascade);
                      
                entity.HasOne(e => e.Author)
                      .WithMany(u => u.Comments)
                      .HasForeignKey(e => e.AuthorId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "Technology",
                    Description = "Latest technology trends and tutorials",
                    Slug = "technology",
                    Color = "#007bff",
                    CreatedDate = DateTime.Now
                },
                new Category
                {
                    Id = 2,
                    Name = "Programming",
                    Description = "Programming tutorials and best practices",
                    Slug = "programming",
                    Color = "#28a745",
                    CreatedDate = DateTime.Now
                },
                new Category
                {
                    Id = 3,
                    Name = "Web Development",
                    Description = "Web development tutorials and frameworks",
                    Slug = "web-development",
                    Color = "#17a2b8",
                    CreatedDate = DateTime.Now
                },
                new Category
                {
                    Id = 4,
                    Name = "General",
                    Description = "General blog posts and announcements",
                    Slug = "general",
                    Color = "#6c757d",
                    CreatedDate = DateTime.Now
                }
            );

            // Seed Users (Note: In production, use proper password hashing service)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@blogsystem.com",
                    PasswordHash = "$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LeWpjDOCHBVrXLOyu", // Admin123!
                    Role = UserRoles.Admin,
                    FirstName = "System",
                    LastName = "Administrator",
                    Bio = "System administrator and main blog author",
                    CreatedDate = DateTime.Now,
                    IsActive = true
                },
                new User
                {
                    Id = 2,
                    Username = "editor",
                    Email = "editor@blogsystem.com",
                    PasswordHash = "$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LeWpjDOCHBVrXLOyu", // Editor123!
                    Role = UserRoles.Editor,
                    FirstName = "Blog",
                    LastName = "Editor",
                    Bio = "Content editor and reviewer",
                    CreatedDate = DateTime.Now,
                    IsActive = true
                },
                new User
                {
                    Id = 3,
                    Username = "johndoe",
                    Email = "john@example.com",
                    PasswordHash = "$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LeWpjDOCHBVrXLOyu", // User123!
                    Role = UserRoles.User,
                    FirstName = "John",
                    LastName = "Doe",
                    Bio = "Technology enthusiast and blogger",
                    CreatedDate = DateTime.Now,
                    IsActive = true
                }
            );

            // Seed Posts
            modelBuilder.Entity<Post>().HasData(
                new Post
                {
                    Id = 1,
                    Title = "Welcome to Our Enhanced Blog System",
                    Content = @"Welcome to our newly enhanced blog system! This platform now features a comprehensive user management system, categorized posts, and an improved commenting system.

## New Features

### User Management
- User registration and authentication
- Role-based access control (Admin, Editor, User)
- User profiles with bio and profile pictures

### Enhanced Posts
- Categories for better organization
- Featured posts highlighting
- SEO-friendly URLs with slugs
- Meta descriptions for better search engine optimization

### Improved Comments
- Support for both registered users and anonymous comments
- Comment moderation system
- Better spam protection

### Better Organization
- Categories with custom colors
- Tag system for cross-referencing content
- Advanced search and filtering options

We hope you enjoy these new features and find them useful for creating and managing your blog content!",
                    Summary = "Introduction to our enhanced blog system with new user management, categories, and improved features.",
                    Slug = "welcome-to-enhanced-blog-system",
                    PublicationDate = DateTime.Now,
                    CreatedDate = DateTime.Now,
                    IsPublished = true,
                    IsFeatured = true,
                    Tags = "welcome, features, blog-system, announcement",
                    AuthorId = 1,
                    CategoryId = 4,
                    MetaDescription = "Welcome to our enhanced blog system with user management, categories, and improved commenting."
                },
                new Post
                {
                    Id = 2,
                    Title = "Getting Started with ASP.NET Core MVC",
                    Content = @"ASP.NET Core MVC is a powerful framework for building modern web applications. In this comprehensive guide, we'll explore the fundamentals of creating a web application with ASP.NET Core MVC.

## What is ASP.NET Core MVC?

ASP.NET Core MVC is a rich framework for building web apps and APIs using the Model-View-Controller design pattern. It provides a way to build dynamic websites that enables a clean separation of concerns.

## Key Features

### Model-View-Controller Pattern
- **Models**: Represent data and business logic
- **Views**: Handle the display of information
- **Controllers**: Handle user input and interaction

### Dependency Injection
ASP.NET Core has built-in support for dependency injection, making your applications more testable and maintainable.

### Cross-Platform
Run on Windows, macOS, and Linux.

## Creating Your First MVC Application

1. Install the .NET SDK
2. Create a new project: `dotnet new mvc`
3. Run the application: `dotnet run`

## Best Practices

- Use proper naming conventions
- Implement proper error handling
- Follow SOLID principles
- Write unit tests
- Use async/await for I/O operations

This is just the beginning of your ASP.NET Core MVC journey. Stay tuned for more advanced topics!",
                    Summary = "A comprehensive introduction to ASP.NET Core MVC covering the basics, key features, and best practices.",
                    Slug = "getting-started-aspnet-core-mvc",
                    PublicationDate = DateTime.Now.AddDays(-1),
                    CreatedDate = DateTime.Now.AddDays(-1),
                    IsPublished = true,
                    IsFeatured = false,
                    Tags = "asp.net, core, mvc, tutorial, web-development",
                    AuthorId = 1,
                    CategoryId = 2,
                    MetaDescription = "Learn ASP.NET Core MVC fundamentals, key features, and best practices for building modern web applications."
                },
                new Post
                {
                    Id = 3,
                    Title = "Modern Web Development with Entity Framework Core",
                    Content = @"Entity Framework Core (EF Core) is a lightweight, extensible, open source and cross-platform version of the popular Entity Framework data access technology.

## What is Entity Framework Core?

EF Core serves as an object-relational mapper (O/RM), which:
- Enables .NET developers to work with a database using .NET objects
- Eliminates the need for most of the data-access code that typically needs to be written

## Key Features

### Code First Approach
Define your model using C# classes and let EF Core create the database schema.

### Database First Approach
Generate your model from an existing database.

### LINQ Support
Write queries using LINQ syntax that gets translated to SQL.

### Migration System
Manage database schema changes over time.

## Getting Started

### 1. Install the Package
```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

### 2. Define Your Models
```csharp
public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
    public List<Post> Posts { get; set; }
}
```

### 3. Create a DbContext
```csharp
public class BloggingContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }
}
```

### 4. Use Your Context
```csharp
using (var context = new BloggingContext())
{
    var blogs = context.Blogs.ToList();
}
```

## Best Practices

- Use async methods for database operations
- Implement proper error handling
- Use migrations for schema changes
- Consider performance implications
- Use proper indexing strategies

EF Core makes data access in .NET applications much more manageable and productive!",
                    Summary = "Learn about Entity Framework Core, its features, and how to use it effectively in modern web development.",
                    Slug = "modern-web-development-entity-framework-core",
                    PublicationDate = DateTime.Now.AddDays(-2),
                    CreatedDate = DateTime.Now.AddDays(-2),
                    IsPublished = true,
                    IsFeatured = true,
                    Tags = "entity-framework, core, orm, database, web-development",
                    AuthorId = 2,
                    CategoryId = 3,
                    MetaDescription = "Comprehensive guide to Entity Framework Core for modern web development with best practices and examples."
                }
            );

            // Seed Comments
            modelBuilder.Entity<Comment>().HasData(
                new Comment
                {
                    Id = 1,
                    Content = "Great introduction to the new blog system! I'm excited to see all these new features.",
                    CommentDate = DateTime.Now.AddHours(-2),
                    CreatedDate = DateTime.Now.AddHours(-2),
                    IsApproved = true,
                    PostId = 1,
                    AuthorId = 3
                },
                new Comment
                {
                    Id = 2,
                    Content = "This is exactly what I was looking for! The MVC pattern explanation is very clear.",
                    CommentDate = DateTime.Now.AddHours(-4),
                    CreatedDate = DateTime.Now.AddHours(-4),
                    IsApproved = true,
                    PostId = 2,
                    AuthorId = 3
                },
                new Comment
                {
                    Id = 3,
                    Content = "Thanks for this comprehensive guide on Entity Framework Core. Very helpful!",
                    CommentDate = DateTime.Now.AddHours(-1),
                    CreatedDate = DateTime.Now.AddHours(-1),
                    IsApproved = true,
                    PostId = 3,
                    AnonymousName = "Anonymous Reader",
                    AnonymousEmail = "reader@example.com"
                }
            );
        }
    }
}
