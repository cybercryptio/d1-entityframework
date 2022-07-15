// Copyright 2020-2022 CYBERCRYPT
using Microsoft.EntityFrameworkCore;
using CyberCrypt.D1.Client;
using System;

namespace CyberCrypt.D1.EntityFramework.Tests.Models;

public class TestDbContext : D1DbContext
{
    public DbSet<EncryptedData> EncryptedData { get; set; } = null!;

    public TestDbContext(Func<ID1Generic> clientFactory, DbContextOptions options)
        : base(clientFactory, options) { }
}

public class EncryptedData
{
    public int Id { get; set; }

    [Confidential]
    public string? Data { get; set; }

    [Confidential]
    public byte[]? Binary { get; set; }
}