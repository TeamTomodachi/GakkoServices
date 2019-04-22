# GakkoServices.Microservices.MetadataService

This service handles static data e.g. teams, pokemon, badges.

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