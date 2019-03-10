# Required Files

Two files named the following are required in this directory:
* secretappsettings.json
* secretappsettings.Development.json

The format of the file is as follows
```
{
  "dbUsername": "",
  "dbPassword": "",
  "dbConnectionString": "",
  "dbServerEngine": "",
}
```

Where `dbServerEngine` is any of the supported values in the Enum located in `Database.cs`. A snippet as of this committed version below
```
public enum SupportedDatabaseServerEngines
{
    None = 0,
    MSSQL = 1,
    Postgresql = 2
}
```

# How to use Template
1. Clone Folder and Rename with proper Service Name
2. Rename the .csproj to match FolderName format
3. Change Namespace in Project Properties to match Folder Name
4. Ctrl+f and change all existing namespaces in project to match new Namespace Structure
5. Rename the DbContext to match Service Name 
6. Go into the `GenerateMigrations.ps1` script and update the `$applicationDbPath` Context with the new Namespace and ContextName
7. Go into `GenerateAllMigrations.ps1` script and add new Migration to `$migrationScriptPaths` 
7. Add to Solution under "Microservices" Solution Folder