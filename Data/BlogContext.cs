using Microsoft.EntityFrameworkCore;
using BlogSystemApp.Models;

namespace BlogSystemApp.Data
{
    public class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions<BlogContext> options) : base(options)
        {
        }

        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure BlogPost entity
            modelBuilder.Entity<BlogPost>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.Author).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Summary).HasMaxLength(500);
                entity.Property(e => e.Tags).HasMaxLength(200);
            });

            // Configure Comment entity
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Author).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Content).IsRequired();
                
                // Configure foreign key relationship
                entity.HasOne(e => e.BlogPost)
                      .WithMany()
                      .HasForeignKey(e => e.BlogPostId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed data
            modelBuilder.Entity<BlogPost>().HasData(
                new BlogPost
                {
                    Id = 1,
                    Title = "Welcome to My Blog",
                    Content = "This is the first post on my new blog system. Welcome to all readers!",
                    Summary = "A welcome message to introduce the blog.",
                    Author = "Admin",
                    CreatedDate = DateTime.Now,
                    IsPublished = true,
                    Tags = "welcome, introduction"
                },
                new BlogPost
                {
                    Id = 2,
                    Title = "Getting Started with ASP.NET Core",
                    Content = "ASP.NET Core is a powerful framework for building modern web applications. In this post, we'll explore the basics of creating a web application with ASP.NET Core MVC.",
                    Summary = "An introduction to ASP.NET Core MVC development.",
                    Author = "Admin",
                    CreatedDate = DateTime.Now.AddDays(-1),
                    IsPublished = true,
                    Tags = "asp.net, core, mvc, tutorial"
                }
            );
        }
    }
}
