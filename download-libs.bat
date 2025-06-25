@echo off
echo Downloading client-side libraries for Blog System...

echo Creating directories...
if not exist "wwwroot\lib\bootstrap\css" mkdir "wwwroot\lib\bootstrap\css"
if not exist "wwwroot\lib\bootstrap\js" mkdir "wwwroot\lib\bootstrap\js"
if not exist "wwwroot\lib\jquery" mkdir "wwwroot\lib\jquery"
if not exist "wwwroot\lib\jquery-validation" mkdir "wwwroot\lib\jquery-validation"
if not exist "wwwroot\lib\jquery-validation-unobtrusive" mkdir "wwwroot\lib\jquery-validation-unobtrusive"

echo Downloading Bootstrap CSS...
curl -o "wwwroot\lib\bootstrap\css\bootstrap.min.css" "https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css"

echo Downloading Bootstrap JS...
curl -o "wwwroot\lib\bootstrap\js\bootstrap.bundle.min.js" "https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"

echo Downloading jQuery...
curl -o "wwwroot\lib\jquery\jquery.min.js" "https://code.jquery.com/jquery-3.6.0.min.js"

echo Downloading jQuery Validation...
curl -o "wwwroot\lib\jquery-validation\jquery.validate.min.js" "https://cdn.jsdelivr.net/npm/jquery-validation@1.19.5/dist/jquery.validate.min.js"

echo Downloading jQuery Validation Unobtrusive...
curl -o "wwwroot\lib\jquery-validation-unobtrusive\jquery.validate.unobtrusive.min.js" "https://cdn.jsdelivr.net/npm/jquery-validation-unobtrusive@3.2.12/dist/jquery.validate.unobtrusive.min.js"

echo Done! All client-side libraries have been downloaded.
pause
