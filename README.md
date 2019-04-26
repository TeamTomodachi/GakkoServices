# GakkoServices

## Routing

The Base URL of the API Gateway is `/auth`

The API Gateway has a single graphical interpreter endpoints

* `/auth/swagger`

And a couple API Controllers

* `/auth/api/Authentication`
* `/auth/api/AuthToken`
* `/auth/api/UserAccount`

The Auth Server also has an IdentityServer4 OAuth Endpoint setup here

* `/auth/account`
* `/auth/.well-known/openid-configuration`

## Configuration

### Generating the Migrations

To generate the migrations, two helper scripts are in each service. `DeleteMigrations.ps1` and `GenerateMigrations.ps1`. These scripts are within the `/Scripts` folder of every Service

Two more helper Scripts are at the root solution `/Scripts` folder `DeleteAllMigrations.ps1` and `GenerateAllMigrations.ps1`. These will go through every Microservice listed within them and call their individual Migration Scripts.

These are Powershell scripts. To use them on Windows, one may need to
[change the execution policy](https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.security/set-executionpolicy?view=powershell-6). On MacOS and Linux, use [Powershell Core](https://github.com/PowerShell/PowerShell).

### SecretAppSettings.json

The following services require a `secretappsettings.json` configuration file in its root. Each of these directories have a file committed called `secretappsettings.example.json` which has an example configuration for that service. 

Each service also contains their own `README.md` containing more information

* AuthServer
* ProfileService
* MetadataService

### Pulling the GakkoFront Submodule

To pull the GakkoFront Submodule, run the following Git Command

```sh
cd GakkoFront
git submodule update --recursive --remote
```

### Docker

#### Installing Docker

GakkoServices supports both Legacy and Modern Docker installations. On Windows, this means:

- Windows 10 Home Edition: Docker Toolbox (Legacy)
- Windows 10 Professional/Enterprise Installations: Docker For Windows

Installation guide for Docker can be found here:

> https://docs.docker.com/docker-for-windows/install/

#### Starting up Docker

From the root GakkoServices directory, run the following commands depending on your scenario

To rebuild all images and start everything:

```sh
docker-compose up --build
```

To rebuild a specific image based on its name in the `docker-compose.yml` file:

```sh
docker-compose build IMAGE_NAME
```

To startup the Docker Containers using the previous built containers

```sh
docker-compose up
```

For much more detail on the Docker setup, see the comments in the [`docker-compose.yml` file](https://github.com/TeamTomodachi/GakkoServices/blob/master/docker-compose.yml).

## Creating a Microservice

To create a Microservice, follow the `README.md` located wthin `GakkoServices.Microservices.ExampleTemplateService`

## Updating the GraphQL Schema

To create a Microservice, follow the `README.md` located wthin `GakkoServices.APIGateway`
