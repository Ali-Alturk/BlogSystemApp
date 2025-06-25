using System.ComponentModel.DataAnnotations;

namespace BlogSystemApp.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        public string Summary { get; set; } = string.Empty;
        
        [Required]
        public string Author { get; set; } = string.Empty;
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        public DateTime? UpdatedDate { get; set; }
        
        public bool IsPublished { get; set; } = false;
        
        public string Tags { get; set; } = string.Empty;
        
        public int ViewCount { get; set; } = 0;
    }
}
