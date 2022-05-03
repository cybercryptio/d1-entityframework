# Encryptonize Entity Framework

## Getting started

### Prerequisites

- The [Encryptonize Core](https://github.com/cyber-crypt-com/encryptonize-core) service must be deployed and accessible. **TODO**: Link to information on how to deploy?
- [Entity Framework Core 6](https://docs.microsoft.com/en-us/ef/core/) must be referenced in the application.
- A supported database deployed. **TODO**: Insert link to list of supported databases, or something else.

### Installation

The Entity Framework Core integration is available through nuget.org. The latest version can be installed using the following command:

```bash
dotnet add package Encryptonize.EntityFramework
```

### Usage

Starting to encrypt a database requires minimal code changes to the an application.

The model needs to told what data should be encrypted. This can be done in two ways:

- Adding the `Confidential` data annotation to the model
- Configuring the model using the fluent API

Once the model have been configured, the chosen data will be encrypted and decrypted transparently, without any additional code changes.

#### Using data annotation

```csharp
using Microsoft.EntityFrameworkCore;
using Encryptonize.EntityFramework;
using Encryptonize.Client;

public class Person
{
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string Surname { get; set; }

    [Confidential] // Confidential data annotation is added to the encrypted property
    public string SocialSecurityNumber { get; set; }
}

public class DatabaseContext : DbContext
{
    private readonly IEncryptonizeClient client;

    public DbSet<Person> Persons { get; set; };

    // An Encryptonize client is injected
    public DatabaseContext(IEncryptonizeClient client, DbContextOptions options) : base(options)
    {
        this.client = client;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // The model is configured to encrypt and decrypt data based on data annotations
        modelBuilder.UseEncryptonize(client);
        base.OnModelCreating(modelBuilder);
    }
}
```

- An instance for `IEncryptonizeClient`, used to communicate with the Encryptonize service, has been injected into the `DbContext`.
- The `Confidential` data annotation is used to mark data that should be encrypted.
- `UseEncryptonize` in `OnModelCreating` is used to tell Entity Framework Core to encrypt and decrypt data transparently.

#### Using the fluent API

```csharp
using Microsoft.EntityFrameworkCore;
using Encryptonize.EntityFramework;
using Encryptonize.Client;

public class Person
{
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string Surname { get; set; }

    public string SocialSecurityNumber { get; set; }
}

public class DatabaseContext : DbContext
{
    private readonly IEncryptonizeClient client;

    public DbSet<Persons> Person { get; set; };

    // An Encryptonize client is injected
    public DatabaseContext(IEncryptonizeClient client, DbContextOptions options) : base(options)
    {
        this.client = client;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Model property is configured to encrypt and decrypt data
        modelBuilder.Entity<Person>().Property(x => x.SocialSecurityNumber).IsConfidential(client);
    }
}
```

- An instance for `IEncryptonizeClient`, used to communicate with the Encryptonize service, has be injected into the `DbContext`.
- The model needs to be told, what data should be encrypted and decrypted, by marking it as confidential using the `IsConfidential` extension method.

### Migrating data

**TODO**: Waiting for the actual migration to be implemented/reviewed/merged.

### Sample

A sample application is available in the [examples directory](https://github.com/cyber-crypt-com/encryptonize-entityframework/tree/master/examples/Encryptonize.SampleDB), showcasing how to use `Encryptonize.EntityFramework` to encrypt and decrypt data in a database.

## How it works

The integration works by encrypted and decrypting data transparently when queried or saved to the database. Selected parts of the data is encrypted from the application to the database in such a way that the database itself never receives the data in plain text.

When data is saved to the database the data will be similarly be encrypted by making a request to the Encryptonize service. An exception is thrown if the application does not have permissions to encrypt data or the Encryptonize service is not available.

**TODO**: Insert diagram

When data is queried, the data will automatically be decrypted by making a request to the Encryptonize service. If the decryption for some reason fails, for example if the application does not have access to the data or the data is corrupted, an exception will be thrown and the data will not be available.

**TODO**: Insert diagram

## API reference

**TODO**: Insert link to API reference.

## Limitations

- Currently only `string` and `byte[]` properties can be encrypted.
- Encrypted data is not searchable by the database.
