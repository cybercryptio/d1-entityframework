# Encryptonize Entity Framework

The integration works by encrypting and decrypting data transparently, using [Encryptonize&reg;](https://github.com/cyber-crypt-com/encryptonize-core/) when querying or storing in the database. Selected parts of the data is encrypted from the application to the database in such a way that the database itself never receives the data in plain text.

This protects the data in the database from being read by third parties and tampering.

## Installation

The Entity Framework Core integration is available through nuget.org. The latest version can be installed using the following command:

```bash
dotnet add package Encryptonize.EntityFramework
```

## Usage

To get started, see the [Getting Started guide](documentation/getting_started.md), and for more in-depth explantaion and usage, see the [User Manual](documentation/user_manual.md).

## API reference

[API Reference](documentation/api/Encryptonize.EntityFramework.md)

## Limitations

- Currently only `string` and `byte[]` properties can be encrypted.
- Encrypted data is not searchable by the database.
