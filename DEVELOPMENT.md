# Development

This document describes the process for building the library on your local computer.

## Getting started

You will need to install .NET 6 to build the project. To install .NET 6 follow the instructions on the [Microsoft website](https://dotnet.microsoft.com/download/dotnet-core/).

## Building the project

The project can be built using the standard `dotnet build` command.

But it is recommended to use the makefile, `make build`, as it ensures API documentation is updated as needed.

To generate the API documentation, XmlDocMarkdown needs to be installed. To install XmlDocMarkdown run `dotnet tool install xmldocmd -g`.
