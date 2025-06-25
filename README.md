# Blog System - ASP.NET Core MVC

A modern blog system built with ASP.NET Core MVC, Entity Framework Core, and SQLite.

## Features

- ✨ Create, edit, and delete blog posts
- 🔍 Search and filter functionality
- 📊 View tracking
- 🏷️ Tag system
- 📱 Responsive design
- 💾 SQLite database integration
- 🎨 Bootstrap UI framework

## Technologies Used

- **Backend:** ASP.NET Core 8.0 MVC
- **Database:** SQLite with Entity Framework Core
- **Frontend:** Bootstrap 5, jQuery
- **Validation:** Client & Server-side validation

## Project Structure

```
BlogSystemApp/
├── Controllers/           # MVC Controllers
│   ├── HomeController.cs
│   └── BlogController.cs
├── Models/               # Data models
│   ├── BlogPost.cs
│   ├── Comment.cs
│   └── ErrorViewModel.cs
├── Views/                # Razor views
│   ├── Home/
│   ├── Blog/
│   └── Shared/
├── Data/                 # Entity Framework context
│   └── BlogContext.cs
├── wwwroot/              # Static files
│   ├── css/
│   ├── js/
│   └── lib/
└── Program.cs            # Application entry point
```

## Setup Instructions

### 1. Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code

### 2. Download Client Libraries
Run the following command to download Bootstrap, jQuery, and validation libraries:
```bash
download-libs.bat
```

### 3. Restore NuGet Packages
```bash
dotnet restore
```

### 4. Create Database
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 5. Run the Application
```bash
dotnet run
```

The application will be available at `https://localhost:5001` or `http://localhost:5000`.

## Database Schema

### BlogPost
- Id (Primary Key)
- Title (Required, Max 200 chars)
- Content (Required)
- Summary (Optional, Max 500 chars)
- Author (Required, Max 100 chars)
- CreatedDate
- UpdatedDate
- IsPublished
- Tags (Comma-separated)
- ViewCount

### Comment
- Id (Primary Key)
- Author (Required)
- Email (Required)
- Content (Required)
- CreatedDate
- IsApproved
- BlogPostId (Foreign Key)

## Key Features

### Blog Management
- Create new blog posts with rich content
- Edit existing posts
- Delete posts with confirmation
- Publish/unpublish posts
- Track view counts

### Search & Navigation
- Search posts by title, content, or tags
- Sort by date or title
- Filter published posts
- Responsive pagination

### User Interface
- Clean, modern Bootstrap design
- Mobile-responsive layout
- Form validation feedback
- Loading states and animations

## Configuration

### Database Connection
The SQLite database connection is configured in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=blog.db"
  }
}
```

### Routing
Default MVC routing is configured:
- Home: `/`
- Blog List: `/Blog`
- Blog Details: `/Blog/Details/{id}`
- Create Post: `/Blog/Create`
- Edit Post: `/Blog/Edit/{id}`

## Development

### Adding New Features
1. Create models in `Models/` folder
2. Update `BlogContext.cs` to include new DbSets
3. Create migrations: `dotnet ef migrations add [MigrationName]`
4. Update database: `dotnet ef database update`
5. Create controllers and views

### Custom Styling
Add custom CSS to `wwwroot/css/site.css`

### JavaScript Enhancements
Add custom JavaScript to `wwwroot/js/site.js`

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## License

This project is open source and available under the MIT License.

## Support

For issues and questions, please create an issue in the repository.
