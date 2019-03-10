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
2. Change Namespace in Project Properties to match Folder Name
3. Ctrl+f and change all existing namespaces in project to match new Namespace Structure
4. Add to Solution under "Microservices" Solution Folder