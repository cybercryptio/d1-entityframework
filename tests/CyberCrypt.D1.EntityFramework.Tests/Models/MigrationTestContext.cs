// Copyright 2020-2022 CYBERCRYPT
using Microsoft.EntityFrameworkCore;
using CyberCrypt.D1.Client;
using System;

namespace CyberCrypt.D1.EntityFramework.Tests.Models;

public class MigrationTestContext : D1DbContext
{
    public DbSet<MigrationData> Data { get; set; } = null!;

    public MigrationTestContext(Func<ID1Generic> clientFactory, DbContextOptions options)
        : base(clientFactory, options) { }
}

public class MigrationData
{
    public int Id { get; set; }

    public string? UnencryptedData { get; set; }

    [Confidential]
    public string? EncryptedData { get; set; }

    public byte[]? UnencryptedBinaryData { get; set; }

    [Confidential]
    public byte[]? EncryptedBinaryData { get; set; }
}