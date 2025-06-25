using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogSystemApp.Models
{
    public class Post
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Summary { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string Slug { get; set; } = string.Empty;
        
        public DateTime PublicationDate { get; set; } = DateTime.Now;
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        public DateTime? UpdatedDate { get; set; }
        
        public bool IsPublished { get; set; } = false;
        
        public bool IsFeatured { get; set; } = false;
        
        public string Tags { get; set; } = string.Empty;
        
        public int ViewCount { get; set; } = 0;
        
        public string FeaturedImage { get; set; } = string.Empty;
        
        public string MetaDescription { get; set; } = string.Empty;
        
        // Foreign keys
        [Required]
        public int AuthorId { get; set; }
        
        public int? CategoryId { get; set; }
        
        // Navigation properties
        [ForeignKey("AuthorId")]
        public virtual User Author { get; set; } = null!;
        
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }
        
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        
        // Computed properties
        public int CommentCount => Comments?.Count(c => c.IsApproved) ?? 0;
        
        public string AuthorName => Author?.DisplayName ?? "Unknown";
        
        public string CategoryName => Category?.Name ?? "Uncategorized";
        
        // Method to generate slug from title
        public void GenerateSlug()
        {
            if (!string.IsNullOrEmpty(Title))
            {
                Slug = Title.ToLower()
                    .Replace(' ', '-')
                    .Replace("'", "")
                    .Replace("\"", "")
                    .Replace(".", "")
                    .Replace(",", "")
                    .Replace("?", "")
                    .Replace("!", "")
                    .Replace("&", "and");
            }
        }
    }
}
