# D1 Entity Framework sample application

## Introduction

The application shows how to use D1 Entity Framework to encrypt and decrypt data in a database. It exposes a simple REST API that encrypts data in a single column in a MSSQL database.

The data schema is automatically created on startup, if it does not already exists, and looks like this:

| Documents        |                                             |
| ---------------- | ------------------------------------------- |
| `Id`             | The ID of the document                      |
| `Data`           | The document data, this column is encrypted |
| `AdditionalData` | Some additional data, not encrypted         |

Through the API it is possible to retrieve documents (using GET requests), create new documents (using POST requests), and delete documents (using DELETE requests).

When a new document is created the values in the `Data` column are automatically encrypted, and when a documented is retrieved it will automatically be decrypted, without any changes to the business logic.

## Running the sample

To run the sample, the application needs to know about the location of the database and the D1 Generic service and how to login into D1.
It can be configured either through the configuration file [appsettings.json](appsettings.json) or environment variables. The configurations that need to be defined before running the application are (here shown as environment variables):

| Variable | Description | Example |
| - | - | - |
| D1DB_CONNECTIONSTRINGS__DEFAULTCONNECTIONSTRING | The database connection string      | `Server=tcp:<insert sqlserver>,1433;Initial Catalog=SampleDB;Persist Security Info=False;User ID=<insert user ID>;Password=<insert user paD1DBd>;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30` |
| D1DB_D1GENERIC__URL                          | The URL to the D1 Generic service | `http://localhost:9000` |
| D1DB_D1GENERIC__USERNAME                     | The D1 Generic username           | `<insert D1 Generic username>` |
| D1DB_D1GENERIC__PASSWORD                     | The D1 Generic password           | `<insert D1 Generic password>` |
| D1DB_D1GENERIC__OIDC__AUTHORIZATIONENDPOINT  | The OIDC authorization endpoint   | `<insert OIDC endpoint>` |
| D1DB_D1GENERIC__OIDC__CLIENTID               | The OIDC client id                | `<insert OIDC client id>` |

Once those environment variables have been defined, the application can be started by running:

```
dotnet run
```

For easy access to the API, the Swagger UI is available at [http://localhost:5000/swagger/index.html](http://localhost:5000/swagger/index.html).

## Kubernetes deployment

If you want to deploy the sample application to Kubernetes a [Helm template](deploy/D1DBSample/) is available.

## Azure provision

Instructions on provisioning the required SQL server and the database are available [here](deploy/README.md).
