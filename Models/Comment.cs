using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogSystemApp.Models
{
    public class Comment
    {
        public int Id { get; set; }
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        public DateTime CommentDate { get; set; } = DateTime.Now;
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        public DateTime? UpdatedDate { get; set; }
        
        public bool IsApproved { get; set; } = false;
        
        public bool IsDeleted { get; set; } = false;
        
        public string UserAgent { get; set; } = string.Empty;
        
        public string IpAddress { get; set; } = string.Empty;
        
        // Foreign keys
        [Required]
        public int PostId { get; set; }
        
        public int? AuthorId { get; set; } // Nullable for anonymous comments
        
        // For anonymous comments (when AuthorId is null)
        [StringLength(100)]
        public string AnonymousName { get; set; } = string.Empty;
        
        [EmailAddress]
        [StringLength(255)]
        public string AnonymousEmail { get; set; } = string.Empty;
        
        [StringLength(255)]
        public string AnonymousWebsite { get; set; } = string.Empty;
        
        // Navigation properties
        [ForeignKey("PostId")]
        public virtual Post Post { get; set; } = null!;
        
        [ForeignKey("AuthorId")]
        public virtual User? Author { get; set; }
        
        // Computed properties
        public string AuthorDisplayName => Author?.DisplayName ?? AnonymousName ?? "Anonymous";
        
        public string AuthorEmail => Author?.Email ?? AnonymousEmail ?? string.Empty;
        
        public bool IsAnonymous => AuthorId == null;
        
        public bool IsRegisteredUser => AuthorId != null;
    }
}
