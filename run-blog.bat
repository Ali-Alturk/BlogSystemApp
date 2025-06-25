@echo off
echo ========================================
echo      Blog System Setup & Run
echo ========================================

echo.
echo Step 1: Restoring NuGet packages...
dotnet restore

echo.
echo Step 2: Building application...
dotnet build

echo.
echo Step 3: Updating database...
dotnet ef database update

echo.
echo Step 4: Starting application...
echo The blog system will be available at:
echo - http://localhost:5000
echo - https://localhost:5001 (if HTTPS is configured)
echo.
echo Press Ctrl+C to stop the application
echo.

dotnet run
