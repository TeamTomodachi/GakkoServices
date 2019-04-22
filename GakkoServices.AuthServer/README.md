# GakkoServices.AuthServer

## Required Files

These files must exist in this directory:

* secretappsettings.json
* secretappsettings.Development.json

The format of each file is as follows:

```json
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

Where `dbServerEngine` is any of the supported values in the Enum located in
`Database.cs`. As of this commit, the enum looks like this:

```c#
public enum SupportedDatabaseServerEngines
{
    None = 0,
    MSSQL = 1,
    Postgresql = 2
}
```

## Docker

The AuthServer is exposed to the internet at `/auth`. It exists next to the
APIGateway. The client accesses it separately from the APIGateway when logging
in, verifying a token, or signing up.