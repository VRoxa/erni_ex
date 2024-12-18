# ERNI technical test

By Víctor de la Rocha (VRoxa).

### How to execute

#### Docker execution

1. Run a postgres container locally.
   ```bash
   $ docker run --name postgresql bitnami/postgresql:latest -d -p 5432:5432 \
   -e POSTGRESQL_USERNAME=postgres \
   -e POSTGRESQL_PASSWORD=postgres
   ```

2. Select the `Container (Dockerfile)` launch profile in Visual Studio.
   Change the connection string environment variable, according to the postgres container, in  the `src/ERNI.Server/Properties/launchSettings.json` file.

   ```json
   "environmentVariables": {
     "ASPNETCORE_HTTPS_PORTS": "8081",
     "ASPNETCORE_HTTP_PORTS": "8080",
     "ConnectionStrings__Database": "Server=host.docker.internal;Port=5432;Database=erni;User Id=postgres;Password=postgres;"
   }
   ```

3. Run the application (in Debug configuration).

#### Service execution

1. Run a postgres container locally.
   ```bash
   $ docker run --name postgresql bitnami/postgresql:latest -d -p 5432:5432 \
   -e POSTGRESQL_USERNAME=postgres \
   -e POSTGRESQL_PASSWORD=postgres
   ```

2. Select the `http` or `https` launch profile in Visual Studio.
   Change the connection string value in the `appSettings.Development.json` file, under the `"ERNI"` property.

   ```json
   "ConnectionStrings": {
     "ERNI": "Server=localhost;Port=5432;Database=erni;User Id=postgres;Password=postgres;"
   }
   ```

3. Run the application (in Debug configuration).

#### Service execution (without Docker)

1. Select the `http` or `https` launch profile in Visual Studio.
2. Set the `"InMemory"` flag to `true` in the `appSettings.Development.json` file.
   This will make the application to run against a non persistent in-memory database.
3. Run the application (in Debug configuration).

### Architecture approach

Slight implementation of the _clean architecture_.  
`ERNI.Entities` represents the domain layer. `ERNI.Core` represents the core application domain business.  
`ERNI.Infrastructure` tackles any infrastructure concern (in that case, only database integration), whilst `ERNI.Server` glues everything together in an ASP.NET application.

`ERNI.Server`, in the presentation layer only handles entities DTOs, which they are mapped to the domain entities in the application layer (`ERNI.Core`) using standard mapping with `Mapster`.  
DTOs are validated on-the-go using `FluentValidations`, with a clear focus on the separation of concerns using the decorator pattern.
The DAL (Data-Access Layer) is also abstracted from the actual infrastructure integration, using the repository pattern; so that brings a clear boundary between the core application domain and the ORM used (Entity Framework). 

### Missing key parts

- Integration testing (WIP).
- Web application (Angular), to present views to the end user and let them interact with data.
- CI/CD pipeline to assert the application and code quality and deploy a final bundle to the public web.