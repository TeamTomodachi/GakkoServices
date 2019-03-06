[CmdletBinding()]
param();

#Script Location
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition;
$rootProjectPath = (get-item $scriptPath).parent.FullName;
$migrationsProjectPath = $rootProjectPath + "\Data\Migrations";
Write-Host "Script Path: $scriptPath";
Write-Host "Root Project Path: $rootProjectPath";
Write-Host "Migrations Path: $migrationsProjectPath";

# Navigate into the root project folder
Set-Location $rootProjectPath;

# Invoke Migrations
# IdentityServer4
$persistedGrantDbPath = $migrationsProjectPath + "\PersistedGrantDb";
$configurationDbPath = $migrationsProjectPath + "\ConfigurationDb";
Write-Host "IdentityServer Persisted Grant Migrations Path: $persistedGrantDbPath";
Write-Host "IdentityServer Configuration Migrations Path: $configurationDbPath";

dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -o $persistedGrantDbPath
dotnet ef migrations add InitialIdentityServerConfigurationDbMigration -c ConfigurationDbContext -o $configurationDbPath

# AspIdentity
$aspIdentityDbPath = $migrationsProjectPath + "\AspIdentity";
Write-Host "AspIdentity Migrations Path: $aspIdentityDbPath";
dotnet ef migrations add InitialASPIdentity --context GakkoServices.AuthServer.Data.Contexts.AspIdentityDbContext -o $aspIdentityDbPath

# Application