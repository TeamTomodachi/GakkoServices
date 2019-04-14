[CmdletBinding()]
param();

#Script Location
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition;
$rootSolutionPath = (get-item $scriptPath).parent.FullName;
Write-Host "Script Path: $scriptPath";
Write-Host "Root Solution Path: $rootSolutionPath";

# Navigate into the root solution folder
Set-Location $rootSolutionPath;

# Create the Migration Scripts Paths
#& "$rootSolutionPath\GakkoServices.AuthServer\Scripts\GenerateMigrations.ps1"; # Example Call
$migrationScriptPaths = @(
    [io.path]::combine($rootSolutionPath, "GakkoServices.AuthServer", "Scripts", "DeleteMigrations.ps1"),
    [io.path]::combine($rootSolutionPath, "GakkoServices.Microservices.ProfileService", "Scripts", "DeleteMigrations.ps1"),
    [io.path]::combine($rootSolutionPath, "GakkoServices.Microservices.MetadataService", "Scripts", "DeleteMigrations.ps1")
);
    
# Execute all Project Migrations
foreach ($path in $migrationScriptPaths) {
    & $path;
}

Set-Location $scriptPath;
