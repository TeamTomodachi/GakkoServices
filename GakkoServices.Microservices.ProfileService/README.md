# GakkoServices.Microservices.ProfileService

This service manages user profiles. These profiles are separate from user
accounts as defined in the AuthServer. Profiles contain a user's Pokemon Go
information.

## Required Files

The following files must exist in this directory:

* secretappsettings.json
* secretappsettings.Development.json

The format of the file is as follows:

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