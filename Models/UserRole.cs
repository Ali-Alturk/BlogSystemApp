using System.ComponentModel.DataAnnotations;

namespace BlogSystemApp.Models
{
    public enum UserRole
    {
        User,
        Editor,
        Admin,
        SuperAdmin
    }
    
    public static class UserRoles
    {
        public const string User = "User";
        public const string Editor = "Editor";
        public const string Admin = "Admin";
        public const string SuperAdmin = "SuperAdmin";
        
        public static List<string> GetAllRoles()
        {
            return new List<string> { User, Editor, Admin, SuperAdmin };
        }
        
        public static bool IsValidRole(string role)
        {
            return GetAllRoles().Contains(role);
        }
    }
}
