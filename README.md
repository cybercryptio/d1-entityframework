# Encryptonize Entity Framework

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
