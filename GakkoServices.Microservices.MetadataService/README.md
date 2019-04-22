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

## Docker

The MetadataService is inaccessible from the internet. Requests for data must go
through the message queue. This means that it has no access to traefik via the
`traefik` network, but is on the `internal` network, which also has RabbitMQ. It
also has access to the MetadataServiceDB container.