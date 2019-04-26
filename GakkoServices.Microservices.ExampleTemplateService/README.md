# GakkoServices.Microservices.ExampleTemplateService

This is a template for creating new microservices.

## Required Files

The following files must exist in this directory:

* secretappsettings.json
* secretappsettings.Development.json

The format of the file is as follows

```json
{
  "dbUsername": "",
  "dbPassword": "",
  "dbConnectionString": "",
  "dbServerEngine": "",
}
```

Where `dbServerEngine` is any of the supported values in the Enum located in
`Database.cs`. A snippet as of this committed version below:

```c#
public enum SupportedDatabaseServerEngines
{
    None = 0,
    MSSQL = 1,
    Postgresql = 2
}
```

## How to use Template

1. Clone Folder and Rename with proper Service Name
1. Rename the .csproj to match FolderName format
1. Change Namespace in Project Properties to match Folder Name
1. Ctrl+f and change all existing namespaces in project to match new Namespace Structure
1. Rename the DbContext to match Service Name
1. Go into the `GenerateMigrations.ps1` script and update the `$applicationDbPath` Context with the new Namespace and ContextName
1. Go into `GenerateAllMigrations.ps1` script and add new Migration to `$migrationScriptPaths` 
1. Add to Solution under "Microservices" Solution Folder
1. Add an entry to the root `docker-compose.yml` following the format used by the other services.