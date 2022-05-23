// Copyright 2020-2022 CYBERCRYPT
using Microsoft.EntityFrameworkCore;
using Encryptonize.Client;

namespace Encryptonize.EntityFramework.Tests.Models;

public class MigrationTestContext : DbContext
{
    private readonly IEncryptonizeCore client;

    public DbSet<MigrationData> Data { get; set; } = null!;

    public MigrationTestContext(IEncryptonizeCore client, DbContextOptions options) : base(options)
    {
        this.client = client;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseEncryptonize(client);
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