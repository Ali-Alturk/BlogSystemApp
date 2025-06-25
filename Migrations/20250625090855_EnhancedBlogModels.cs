using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BlogSystemApp.Migrations
{
    /// <inheritdoc />
    public partial class EnhancedBlogModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Color = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Bio = table.Column<string>(type: "TEXT", nullable: false),
                    ProfilePicture = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    Summary = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PublicationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsPublished = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsFeatured = table.Column<bool>(type: "INTEGER", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ViewCount = table.Column<int>(type: "INTEGER", nullable: false),
                    FeaturedImage = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    MetaDescription = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    AuthorId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Posts_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    CommentDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsApproved = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserAgent = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 45, nullable: false),
                    PostId = table.Column<int>(type: "INTEGER", nullable: false),
                    AuthorId = table.Column<int>(type: "INTEGER", nullable: true),
                    AnonymousName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AnonymousEmail = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    AnonymousWebsite = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Color", "CreatedDate", "Description", "IsActive", "Name", "Slug" },
                values: new object[,]
                {
                    { 1, "#007bff", new DateTime(2025, 6, 25, 12, 8, 53, 168, DateTimeKind.Local).AddTicks(640), "Latest technology trends and tutorials", true, "Technology", "technology" },
                    { 2, "#28a745", new DateTime(2025, 6, 25, 12, 8, 53, 168, DateTimeKind.Local).AddTicks(645), "Programming tutorials and best practices", true, "Programming", "programming" },
                    { 3, "#17a2b8", new DateTime(2025, 6, 25, 12, 8, 53, 168, DateTimeKind.Local).AddTicks(648), "Web development tutorials and frameworks", true, "Web Development", "web-development" },
                    { 4, "#6c757d", new DateTime(2025, 6, 25, 12, 8, 53, 168, DateTimeKind.Local).AddTicks(651), "General blog posts and announcements", true, "General", "general" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Bio", "CreatedDate", "Email", "FirstName", "IsActive", "LastLoginDate", "LastName", "PasswordHash", "ProfilePicture", "Role", "Username" },
                values: new object[,]
                {
                    { 1, "System administrator and main blog author", new DateTime(2025, 6, 25, 12, 8, 53, 168, DateTimeKind.Local).AddTicks(1271), "admin@blogsystem.com", "System", true, null, "Administrator", "$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LeWpjDOCHBVrXLOyu", "", "Admin", "admin" },
                    { 2, "Content editor and reviewer", new DateTime(2025, 6, 25, 12, 8, 53, 168, DateTimeKind.Local).AddTicks(1276), "editor@blogsystem.com", "Blog", true, null, "Editor", "$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LeWpjDOCHBVrXLOyu", "", "Editor", "editor" },
                    { 3, "Technology enthusiast and blogger", new DateTime(2025, 6, 25, 12, 8, 53, 168, DateTimeKind.Local).AddTicks(1280), "john@example.com", "John", true, null, "Doe", "$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LeWpjDOCHBVrXLOyu", "", "User", "johndoe" }
                });

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "AuthorId", "CategoryId", "Content", "CreatedDate", "FeaturedImage", "IsFeatured", "IsPublished", "MetaDescription", "PublicationDate", "Slug", "Summary", "Tags", "Title", "UpdatedDate", "ViewCount" },
                values: new object[,]
                {
                    { 1, 1, 4, "Welcome to our newly enhanced blog system! This platform now features a comprehensive user management system, categorized posts, and an improved commenting system.\r\n\r\n## New Features\r\n\r\n### User Management\r\n- User registration and authentication\r\n- Role-based access control (Admin, Editor, User)\r\n- User profiles with bio and profile pictures\r\n\r\n### Enhanced Posts\r\n- Categories for better organization\r\n- Featured posts highlighting\r\n- SEO-friendly URLs with slugs\r\n- Meta descriptions for better search engine optimization\r\n\r\n### Improved Comments\r\n- Support for both registered users and anonymous comments\r\n- Comment moderation system\r\n- Better spam protection\r\n\r\n### Better Organization\r\n- Categories with custom colors\r\n- Tag system for cross-referencing content\r\n- Advanced search and filtering options\r\n\r\nWe hope you enjoy these new features and find them useful for creating and managing your blog content!", new DateTime(2025, 6, 25, 12, 8, 53, 168, DateTimeKind.Local).AddTicks(1346), "", true, true, "Welcome to our enhanced blog system with user management, categories, and improved commenting.", new DateTime(2025, 6, 25, 12, 8, 53, 168, DateTimeKind.Local).AddTicks(1345), "welcome-to-enhanced-blog-system", "Introduction to our enhanced blog system with new user management, categories, and improved features.", "welcome, features, blog-system, announcement", "Welcome to Our Enhanced Blog System", null, 0 },
                    { 2, 1, 2, "ASP.NET Core MVC is a powerful framework for building modern web applications. In this comprehensive guide, we'll explore the fundamentals of creating a web application with ASP.NET Core MVC.\r\n\r\n## What is ASP.NET Core MVC?\r\n\r\nASP.NET Core MVC is a rich framework for building web apps and APIs using the Model-View-Controller design pattern. It provides a way to build dynamic websites that enables a clean separation of concerns.\r\n\r\n## Key Features\r\n\r\n### Model-View-Controller Pattern\r\n- **Models**: Represent data and business logic\r\n- **Views**: Handle the display of information\r\n- **Controllers**: Handle user input and interaction\r\n\r\n### Dependency Injection\r\nASP.NET Core has built-in support for dependency injection, making your applications more testable and maintainable.\r\n\r\n### Cross-Platform\r\nRun on Windows, macOS, and Linux.\r\n\r\n## Creating Your First MVC Application\r\n\r\n1. Install the .NET SDK\r\n2. Create a new project: `dotnet new mvc`\r\n3. Run the application: `dotnet run`\r\n\r\n## Best Practices\r\n\r\n- Use proper naming conventions\r\n- Implement proper error handling\r\n- Follow SOLID principles\r\n- Write unit tests\r\n- Use async/await for I/O operations\r\n\r\nThis is just the beginning of your ASP.NET Core MVC journey. Stay tuned for more advanced topics!", new DateTime(2025, 6, 24, 12, 8, 53, 168, DateTimeKind.Local).AddTicks(1358), "", false, true, "Learn ASP.NET Core MVC fundamentals, key features, and best practices for building modern web applications.", new DateTime(2025, 6, 24, 12, 8, 53, 168, DateTimeKind.Local).AddTicks(1353), "getting-started-aspnet-core-mvc", "A comprehensive introduction to ASP.NET Core MVC covering the basics, key features, and best practices.", "asp.net, core, mvc, tutorial, web-development", "Getting Started with ASP.NET Core MVC", null, 0 },
                    { 3, 2, 3, "Entity Framework Core (EF Core) is a lightweight, extensible, open source and cross-platform version of the popular Entity Framework data access technology.\r\n\r\n## What is Entity Framework Core?\r\n\r\nEF Core serves as an object-relational mapper (O/RM), which:\r\n- Enables .NET developers to work with a database using .NET objects\r\n- Eliminates the need for most of the data-access code that typically needs to be written\r\n\r\n## Key Features\r\n\r\n### Code First Approach\r\nDefine your model using C# classes and let EF Core create the database schema.\r\n\r\n### Database First Approach\r\nGenerate your model from an existing database.\r\n\r\n### LINQ Support\r\nWrite queries using LINQ syntax that gets translated to SQL.\r\n\r\n### Migration System\r\nManage database schema changes over time.\r\n\r\n## Getting Started\r\n\r\n### 1. Install the Package\r\n```bash\r\ndotnet add package Microsoft.EntityFrameworkCore.SqlServer\r\n```\r\n\r\n### 2. Define Your Models\r\n```csharp\r\npublic class Blog\r\n{\r\n    public int BlogId { get; set; }\r\n    public string Url { get; set; }\r\n    public List<Post> Posts { get; set; }\r\n}\r\n```\r\n\r\n### 3. Create a DbContext\r\n```csharp\r\npublic class BloggingContext : DbContext\r\n{\r\n    public DbSet<Blog> Blogs { get; set; }\r\n    public DbSet<Post> Posts { get; set; }\r\n}\r\n```\r\n\r\n### 4. Use Your Context\r\n```csharp\r\nusing (var context = new BloggingContext())\r\n{\r\n    var blogs = context.Blogs.ToList();\r\n}\r\n```\r\n\r\n## Best Practices\r\n\r\n- Use async methods for database operations\r\n- Implement proper error handling\r\n- Use migrations for schema changes\r\n- Consider performance implications\r\n- Use proper indexing strategies\r\n\r\nEF Core makes data access in .NET applications much more manageable and productive!", new DateTime(2025, 6, 23, 12, 8, 53, 168, DateTimeKind.Local).AddTicks(1366), "", true, true, "Comprehensive guide to Entity Framework Core for modern web development with best practices and examples.", new DateTime(2025, 6, 23, 12, 8, 53, 168, DateTimeKind.Local).AddTicks(1365), "modern-web-development-entity-framework-core", "Learn about Entity Framework Core, its features, and how to use it effectively in modern web development.", "entity-framework, core, orm, database, web-development", "Modern Web Development with Entity Framework Core", null, 0 }
                });

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "Id", "AnonymousEmail", "AnonymousName", "AnonymousWebsite", "AuthorId", "CommentDate", "Content", "CreatedDate", "IpAddress", "IsApproved", "IsDeleted", "PostId", "UpdatedDate", "UserAgent" },
                values: new object[,]
                {
                    { 1, "", "", "", 3, new DateTime(2025, 6, 25, 10, 8, 53, 168, DateTimeKind.Local).AddTicks(1419), "Great introduction to the new blog system! I'm excited to see all these new features.", new DateTime(2025, 6, 25, 10, 8, 53, 168, DateTimeKind.Local).AddTicks(1421), "", true, false, 1, null, "" },
                    { 2, "", "", "", 3, new DateTime(2025, 6, 25, 8, 8, 53, 168, DateTimeKind.Local).AddTicks(1428), "This is exactly what I was looking for! The MVC pattern explanation is very clear.", new DateTime(2025, 6, 25, 8, 8, 53, 168, DateTimeKind.Local).AddTicks(1429), "", true, false, 2, null, "" },
                    { 3, "reader@example.com", "Anonymous Reader", "", null, new DateTime(2025, 6, 25, 11, 8, 53, 168, DateTimeKind.Local).AddTicks(1432), "Thanks for this comprehensive guide on Entity Framework Core. Very helpful!", new DateTime(2025, 6, 25, 11, 8, 53, 168, DateTimeKind.Local).AddTicks(1433), "", true, false, 3, null, "" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Slug",
                table: "Categories",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AuthorId",
                table: "Comments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId",
                table: "Comments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_AuthorId",
                table: "Posts",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CategoryId",
                table: "Posts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Slug",
                table: "Posts",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
