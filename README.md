# CYBERCRYPT D1 Entity Framework

The integration works by encrypting and decrypting data transparently, using [CYBERCRYPT D1 Generic](https://github.com/cybercryptio/d1-service-generic/) when querying or storing in the database. Selected parts of the data is encrypted from the application to the database in such a way that the database itself never receives the data in plain text.

Searchable encryption is also supported by adding keywords to a secure index handled by CYBERCRYPT D1 Generic. For more information about searchable encryption read [User maanual - Searchable encryption](documentation/user_manual.md#searchable-encryption).

This protects the data in the database from being read by third parties and tampering.

## Supported databases

All databases supported by Entity Framework Core are supported by CyberCrypt.D1.EntityFramework.

Some of the most used databases supported includes:

- Microsoft SQL Server
- Oracle
- MySQL
- MariaDB
- PostgresSQL
- SQLite

## Installation

The Entity Framework Core integration is available through nuget.org. The latest version can be installed using the following command:

```bash
dotnet add package CyberCrypt.D1.EntityFramework
```

## Usage

To get started, see the [Getting Started guide](documentation/getting_started.md), and for more in-depth explantaion and usage, see the [User Manual](documentation/user_manual.md).

## API reference

[API Reference](documentation/api/CyberCrypt.D1.EntityFramework.md)

## Limitations

- Currently only `string` and `byte[]` properties can be encrypted.
