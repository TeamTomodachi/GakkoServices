[CmdletBinding()]
param();

#Script Location
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition;
$rootProjectPath = (get-item $scriptPath).parent.FullName;
$migrationsProjectPath = [io.path]::combine($rootProjectPath, "Data", "Migrations"); #Join-Path $rootProjectPath "Data" "Migrations";
Write-Host "Script Path: $scriptPath";
Write-Host "Root Project Path: $rootProjectPath";
Write-Host "Migrations Path: $migrationsProjectPath";

# Create folder if not exists
New-Item -ItemType Directory -Force -Path $migrationsProjectPath

# Navigate into the root project folder
Set-Location $rootProjectPath;

# Invoke Migrations
# Application
$applicationDbPath = [io.path]::combine($migrationsProjectPath, "Application");
Write-Host "Application Migrations Path: $applicationDbPath";
dotnet ef migrations add InitialApplicationMigration --context GakkoServices.Microservices.MetadataService.Data.Contexts.MetadataServiceDbContext -o $applicationDbPath

