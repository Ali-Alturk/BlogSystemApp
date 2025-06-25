using System.ComponentModel.DataAnnotations;

namespace BlogSystemApp.Models
{
    public class Comment
    {
        public int Id { get; set; }
        
        [Required]
        public string Author { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        public bool IsApproved { get; set; } = false;
        
        // Foreign key
        public int BlogPostId { get; set; }
        
        // Navigation property
        public BlogPost? BlogPost { get; set; }
    }
}
