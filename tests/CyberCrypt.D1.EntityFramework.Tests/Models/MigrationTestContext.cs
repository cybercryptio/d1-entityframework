// Copyright 2020-2022 CYBERCRYPT
using Microsoft.EntityFrameworkCore;
using CyberCrypt.D1.Client;

namespace CyberCrypt.D1.EntityFramework.Tests.Models;

public class MigrationTestContext : DbContext
{
    private readonly ID1Generic client;

    public DbSet<MigrationData> Data { get; set; } = null!;

    public MigrationTestContext(ID1Generic client, DbContextOptions options) : base(options)
    {
        this.client = client;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseD1(client);
        base.OnModelCreating(modelBuilder);
    }
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