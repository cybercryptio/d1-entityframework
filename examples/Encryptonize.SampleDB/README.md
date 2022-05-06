# Encryptonize DB sample application

## Introduction

The application shows how to use Encryptonize DB to encrypt and decrypt data in a database. It exposes a simple REST API that encrypts data in a single column in a MSSQL database.

The data schema is automatically created on startup, if it does not already exists, and looks like this:

| Documents        |                                             |
| ---------------- | ------------------------------------------- |
| `Id`             | The ID of the document                      |
| `Data`           | The document data, this column is encrypted |
| `AdditionalData` | Some additional data, not encrypted         |

Through the API it is possible to retrive documents, create new documents and delete documents.

When a new document is created the values in the `Data` column are automatically encrypted, and when a documented is retrived it will automatically be decrypted, without any changes to the business logic.

## Running the sample

To run the sample, the application needs to know about the location of the database and the Encryptonize service and how to login into Encryptonize.
It can be configured either through the configuration file or environment variables. The configurations that need to be defined before running the application are (here shown as environment variables):

| Variable | Description | Example |
| - | - | - |
| ENCDB_CONNECTIONSTRINGS__DEFAULTCONNECTIONSTRING | The database connection string      | `Server=tcp:sqlserver,1433;Initial Catalog=SampleDB;Persist Security Info=False;User ID=XXXXX;Password=XXXXX;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30` |
| ENCDB_ENCRYPTONIZE__URL                          | The URL to the Encryptonize service | `http://localhost:9000` |
| ENCDB_ENCRYPTONIZE__USERNAME                     | The Encryptonize username           | `974e6844-591f-4c77-bac8-d1e32ba6e159` |
| ENCDB_ENCRYPTONIZE__PASSWORD                     | The Encryptonize password           | `XXXXXXXXXXXXXXXXXXXXXX` |

Once those environment variables have been defined, the application can be started by running:

```
dotnet run
```

For easy access to the API, the Swagger UI is available at [http://localhost:5000/swagger/index.html](http://localhost:5000/swagger/index.html).

## Kubernetes deployment

If you want to deploy the sample application to Kubernetes a [Helm template](../../../deploy/encryptonizedb/EncryptonizeDBSample/) is available.

## Azure provision

Instructions on provisioning the required SQL server and the database are available [here](../../../deploy/encryptonizedb/README.md).