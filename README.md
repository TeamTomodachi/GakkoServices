# GakkoServices

## Configuration

### Generating the Migrations

To generate the migrations, two helper scripts are provided in each service. `DeleteMigrations.ps1` and `GenerateMigrations.ps1`. These scripts are located within the `/Scripts` folder of every Service

In addition, two helper Scripts are provided at the root solution `/Scripts` folder `DeleteAllMigrations.ps1` and `GenerateAllMigrations.ps1`. These will go through every Microservice listed within them and call their individual Migration Scripts

### SecretAppSettings.json

The following services require a `secretappsettings.json` configuration file in its root. Each of these directories have a file committed called `secretappsettings.example.json` which has an example configuration for that service. 

In addition, each service contains their own `README.md` containing additional information

* AuthServer
* ProfileService
* MetadataService

### Pulling the GakkoFront Submodule

To pull the GakkoFront Submodule, run the following Git Command

> `cd GakkoFront`
> `git submodule update --recursive --remote`

### Starting up Docker

From the root GakkoServices directory, run the following commands depending on your scenario

To build the latest changes and startup the Docker Containers:

> `docker-compose up --build`

To startup the Docker Containers using the previous built containers

> `docker-compose up`

## Creating a Microservice

To create a Microservice, follow the `README.md` located wthin `GakkoServices.Microservices.ExampleTemplateService`