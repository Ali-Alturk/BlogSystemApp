using System.ComponentModel.DataAnnotations;

namespace BlogSystemApp.Models.ViewModels
{
    public class BlogIndexViewModel
    {
        public IEnumerable<Post> Posts { get; set; } = new List<Post>();
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public IEnumerable<Post> FeaturedPosts { get; set; } = new List<Post>();
        public IEnumerable<Post> RecentPosts { get; set; } = new List<Post>();
        
        public string? SearchTerm { get; set; }
        public string? SortOrder { get; set; }
        public int? CategoryId { get; set; }
        public string? Tag { get; set; }
        
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPosts { get; set; } = 0;
        
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
    
    public class PostDetailsViewModel
    {
        public Post Post { get; set; } = null!;
        public IEnumerable<Comment> Comments { get; set; } = new List<Comment>();
        public IEnumerable<Post> RelatedPosts { get; set; } = new List<Post>();
        public CommentCreateViewModel NewComment { get; set; } = new CommentCreateViewModel();
    }
    
    public class CommentCreateViewModel
    {
        [Required]
        public string Content { get; set; } = string.Empty;
        
        public int PostId { get; set; }
        
        // For anonymous comments
        [StringLength(100)]
        public string? AnonymousName { get; set; }
        
        [EmailAddress]
        [StringLength(255)]
        public string? AnonymousEmail { get; set; }
        
        [Url]
        [StringLength(255)]
        public string? AnonymousWebsite { get; set; }
    }
}
