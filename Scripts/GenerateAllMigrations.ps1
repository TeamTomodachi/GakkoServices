[CmdletBinding()]
param();

#Script Location
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition;
$rootSolutionPath = (get-item $scriptPath).parent.FullName;
Write-Host "Script Path: $scriptPath";
Write-Host "Root Solution Path: $rootSolutionPath";

# Navigate into the root solution folder
Set-Location $rootSolutionPath;

# Execute all Project Migrations
& "$rootSolutionPath\GakkoServices.AuthServer\Scripts\GenerateMigrations.ps1"
