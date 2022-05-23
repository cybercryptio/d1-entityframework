# Getting started

## Prerequisites

- The [Encryptonize&reg; Core](https://github.com/cyber-crypt-com/encryptonize-core) service must be deployed and accessible. See the [Encryptonize&reg; Core README](https://github.com/cyber-crypt-com/encryptonize-core/blob/master/README.md) for more information.
- [Entity Framework Core 6](https://docs.microsoft.com/en-us/ef/core/) must be referenced in the application.
- [A supported database](supported_databases.md) deployed.

## Installation

The Entity Framework Core integration is available through nuget.org. The latest version can be installed using the following command:

```bash
dotnet add package Encryptonize.EntityFramework
```

## Usage

### Configue data context

The `DbContext` needs to be configured to use the Encryptonize&reg; integration, by overriding the `OnModelCreating` method:

```csharp
using Microsoft.EntityFrameworkCore;
using Encryptonize.EntityFramework;
using Encryptonize.Client;

public class DatabaseContext : DbContext
{
    private readonly IEncryptonizeCore client;

    public DbSet<Person> Persons { get; set; };

    // An Encryptonize client is injected
    public DatabaseContext(IEncryptonizeCore client)
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

The example above uses the data annotations based approach, if you want to use the fluent API instead, please see the [user manual](user_manual.md).

### Add data annotation to model

The final step is to add the `Confidential` data annotation to the model.

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
```

### Storing data

Storing data, is done the same way as with the regular Entity Framework Core.

Before the data is sent to the database, it will be encrypted using the Encryptonize&reg; service, without any additional steps.

```csharp
var person = new Person { Firstname = "John", Surname = "Doe", SocialSecurityNumber = "123456789" };
await dbContext.Persons.AddAsync(person);
await dbContext.SaveChangesAsync();
```

### Query data

Querying data is done the same way as with the regular Entity Framework Core.

When the data is received from the database, it will automatically be decrypted using the Encryptonize&reg; service, you won't need to do anything special to read the data.

```csharp
var person = await dbContext.Documents.FirstOrDefaultAsync(x => x.Firstname == "John");
```
