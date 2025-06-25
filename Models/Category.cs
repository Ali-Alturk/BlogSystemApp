using System.ComponentModel.DataAnnotations;

namespace BlogSystemApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string Slug { get; set; } = string.Empty;
        
        public string Color { get; set; } = "#6c757d"; // Bootstrap color for UI
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
        
        // Computed property for post count
        public int PostCount => Posts?.Count(p => p.IsPublished) ?? 0;
    }
}
