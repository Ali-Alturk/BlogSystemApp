# Enhanced Blog System - ASP.NET Core MVC

A comprehensive blog system built with ASP.NET Core MVC, Entity Framework Core, and SQLite featuring user management, categories, and advanced commenting.

## ✨ New Features

- **👥 User Management**: Complete user system with roles (Admin, Editor, User)
- **📁 Categories**: Organize posts with colored categories
- **⭐ Featured Posts**: Highlight important content
- **💬 Enhanced Comments**: Support for both registered and anonymous users
- **🔍 Advanced Search**: Filter by category, tags, and search terms
- **📊 Analytics**: View tracking and comment counts
- **� Modern UI**: Bootstrap 5 with responsive design

## 🏗️ Enhanced Architecture

### Models

#### User
- Complete user management with authentication
- Role-based access control (Admin, Editor, User)
- Profile information (Bio, Profile Picture)
- Password hashing with BCrypt

#### Post (Enhanced)
- Author relationship (Foreign Key to User)
- Category relationship (Foreign Key to Category)
- SEO-friendly slugs
- Featured post capability
- Meta descriptions
- Publication and update tracking

#### Comment (Enhanced)
- Support for registered users and anonymous comments
- Comment moderation system
- IP tracking and User Agent logging
- Approval workflow

#### Category
- Custom color coding
- Slug-based URLs
- Post count tracking
- Active/inactive states

### Database Schema

```
Users (Authentication & Profiles)
├── Id, Username, Email, PasswordHash
├── Role, FirstName, LastName, Bio
└── ProfilePicture, CreatedDate, IsActive

Categories (Content Organization)
├── Id, Name, Description, Slug
├── Color, CreatedDate, IsActive
└── Posts (Navigation)

Posts (Enhanced Blog Posts)
├── Id, Title, Content, Summary, Slug
├── PublicationDate, CreatedDate, UpdatedDate
├── IsPublished, IsFeatured, Tags, ViewCount
├── FeaturedImage, MetaDescription
├── AuthorId (FK to Users)
├── CategoryId (FK to Categories)
├── Author (Navigation)
├── Category (Navigation)
└── Comments (Navigation)

Comments (Enhanced Commenting)
├── Id, Content, CommentDate, CreatedDate
├── IsApproved, IsDeleted, UserAgent, IpAddress
├── PostId (FK to Posts), AuthorId (FK to Users)
├── AnonymousName, AnonymousEmail, AnonymousWebsite
├── Post (Navigation)
└── Author (Navigation)
```

## 🚀 Setup Instructions

### 1. Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code

### 2. Clone and Setup
```bash
git clone <repository-url>
cd BlogSystemApp
```

### 3. Install Dependencies
```bash
# Restore NuGet packages
dotnet restore

# Download client-side libraries
./download-libs.bat
```

### 4. Database Setup
```bash
# Create initial migration (if not exists)
dotnet ef migrations add EnhancedBlogModels

# Create and seed database
dotnet ef database update
```

### 5. Run Application
```bash
dotnet run
```

The application will be available at `http://localhost:5000`

## 👤 Default Users

The system comes with pre-seeded users for testing:

| Username | Email | Password | Role |
|----------|-------|----------|------|
| admin | admin@blogsystem.com | Admin123! | Admin |
| editor | editor@blogsystem.com | Editor123! | Editor |
| johndoe | john@example.com | User123! | User |

## 📁 Project Structure

```
BlogSystemApp/
├── Controllers/           # MVC Controllers
│   ├── HomeController.cs
│   ├── BlogController.cs
│   └── CommentController.cs
├── Models/               # Enhanced Data Models
│   ├── User.cs
│   ├── Post.cs
│   ├── Comment.cs
│   ├── Category.cs
│   ├── UserRole.cs
│   └── ViewModels/
├── Views/                # Enhanced Razor Views
│   ├── Home/
│   ├── Blog/
│   └── Shared/
├── Data/                 # Entity Framework Context
│   └── BlogContext.cs
├── Services/             # Business Services
│   └── PasswordHashingService.cs
├── wwwroot/              # Static Files
│   ├── css/
│   ├── js/
│   └── lib/
└── Migrations/           # EF Core Migrations
```

## 🎯 Key Features

### Content Management
- ✅ Create, edit, delete posts with rich content
- ✅ Category-based organization
- ✅ Featured posts highlighting
- ✅ SEO-friendly URLs and meta descriptions
- ✅ Tag-based categorization
- ✅ Draft and published states

### User Experience
- ✅ Advanced search and filtering
- ✅ Pagination for large content sets
- ✅ Responsive design for all devices
- ✅ Related posts suggestions
- ✅ View count tracking

### Comment System
- ✅ Registered user comments
- ✅ Anonymous comment support
- ✅ Comment moderation
- ✅ Spam protection with IP tracking

### Administration
- ✅ Role-based access control
- ✅ User management
- ✅ Category management
- ✅ Comment moderation

## 🔧 Configuration

### Database Connection
Configure in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=blog.db"
  }
}
```

### User Roles
The system supports four user roles:
- **SuperAdmin**: Full system access
- **Admin**: Content and user management
- **Editor**: Content creation and editing
- **User**: Basic content viewing and commenting

## 🛠️ Development

### Adding New Features

1. **Models**: Add new entities in `Models/` folder
2. **Database**: Update `BlogContext.cs` and create migrations
3. **Controllers**: Implement business logic
4. **Views**: Create user interfaces
5. **Services**: Add business services as needed

### Database Operations
```bash
# Add new migration
dotnet ef migrations add [MigrationName]

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

### Custom Styling
- Main styles: `wwwroot/css/site.css`
- Custom JavaScript: `wwwroot/js/site.js`

## 📊 Sample Data

The application includes comprehensive seed data:
- **4 Categories**: Technology, Programming, Web Development, General
- **3 Users**: Admin, Editor, and regular User
- **3 Sample Posts**: With rich content and proper categorization
- **3 Sample Comments**: Both registered and anonymous examples

## 🔒 Security Features

- **Password Hashing**: BCrypt with salt rounds
- **SQL Injection Protection**: Entity Framework parameterized queries
- **XSS Protection**: Razor view encoding
- **CSRF Protection**: Anti-forgery tokens
- **Input Validation**: Client and server-side validation

## 🚀 Performance Optimizations

- **Database Indexing**: Optimized queries with proper indexes
- **Lazy Loading**: Efficient data loading patterns
- **Caching**: Static file caching and optimization
- **Responsive Images**: Optimized image delivery

## 📱 Mobile Support

- Fully responsive Bootstrap 5 design
- Touch-friendly interface
- Mobile-optimized navigation
- Fast loading on mobile networks

## 🧪 Testing

### Manual Testing Scenarios
1. **User Registration/Login**: Test authentication flows
2. **Content Creation**: Create posts with different users
3. **Comment System**: Test both anonymous and registered comments
4. **Search/Filter**: Test various search combinations
5. **Category Management**: Test category-based organization

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Implement your changes
4. Add appropriate tests
5. Submit a pull request

## 📄 License

This project is open source and available under the MIT License.

## 📞 Support

For issues and questions, please create an issue in the repository or contact the development team.

---

**Happy Blogging! 🎉**
