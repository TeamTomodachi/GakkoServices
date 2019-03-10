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
  
  "ExternalAuthenticationProviders": {
    "Google": {
      "ClientId": "<insert here>",
      "ClientSecret": "<insert here>"
    },
    "Discord": {
      "ClientId": "<insert here>",
      "ClientSecret": "<insert here>"
    }
  }
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