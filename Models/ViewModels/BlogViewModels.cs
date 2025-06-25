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
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public int TotalComments { get; set; }
        public int ApprovedComments { get; set; }
    }
    
    public class PostCreateEditViewModel
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Summary { get; set; }
        
        [StringLength(500)]
        public string? Tags { get; set; }
        
        public bool IsFeatured { get; set; }
        public bool IsPublished { get; set; }
        
        [Display(Name = "Category")]
        public int? CategoryId { get; set; }
        
        [Display(Name = "Featured Image URL")]
        [Url]
        public string? FeaturedImage { get; set; }
        
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        
        public DateTime? PublicationDate { get; set; }
        public int ViewCount { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
    
    public class CommentCreateViewModel
    {
        [Required]
        [StringLength(2000)]
        public string Content { get; set; } = string.Empty;
        
        public int PostId { get; set; }
        
        // For anonymous comments
        [StringLength(100)]
        [Display(Name = "Name")]
        public string? AnonymousName { get; set; }
        
        [EmailAddress]
        [StringLength(255)]
        [Display(Name = "Email")]
        public string? AnonymousEmail { get; set; }
        
        [Url]
        [StringLength(255)]
        [Display(Name = "Website")]
        public string? AnonymousWebsite { get; set; }
        
        public string PostTitle { get; set; } = string.Empty;
    }
    
    public class CategoryManagementViewModel
    {
        public Category Category { get; set; } = null!;
        public int PostCount { get; set; }
        public int PublishedPostCount { get; set; }
        public IEnumerable<Post> RecentPosts { get; set; } = new List<Post>();
        public bool CanDelete { get; set; }
    }
    
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalPosts { get; set; }
        public int PublishedPosts { get; set; }
        public int DraftPosts { get; set; }
        public int TotalComments { get; set; }
        public int PendingComments { get; set; }
        public int TotalCategories { get; set; }
        public int TotalViews { get; set; }
        
        public IEnumerable<Post> RecentPosts { get; set; } = new List<Post>();
        public IEnumerable<Comment> RecentComments { get; set; } = new List<Comment>();
        public IEnumerable<User> RecentUsers { get; set; } = new List<User>();
        
        // Statistics for charts
        public Dictionary<string, int> PostsByCategory { get; set; } = new();
        public Dictionary<DateTime, int> PostsByDate { get; set; } = new();
        public Dictionary<string, int> CommentsByStatus { get; set; } = new();
    }
}
