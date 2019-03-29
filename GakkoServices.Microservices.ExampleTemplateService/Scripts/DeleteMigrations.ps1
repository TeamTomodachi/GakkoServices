[CmdletBinding()]
param();

#Script Location
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition;
$rootProjectPath = (get-item $scriptPath).parent.FullName;
$migrationsProjectPath = [io.path]::combine($rootProjectPath, "Data", "Migrations");
Write-Host "Script Path: $scriptPath";
Write-Host "Root Project Path: $rootProjectPath";
Write-Host "Migrations Path: $migrationsProjectPath";

# Delete the existing migrations path
Remove-Item -path "$migrationsProjectPath" -Recurse;

# Create folder if not exists
New-Item -ItemType Directory -Force -Path $migrationsProjectPath;
