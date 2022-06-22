// Copyright 2020-2022 CYBERCRYPT
using Microsoft.EntityFrameworkCore;
using CyberCrypt.D1.Client;

namespace CyberCrypt.D1.EntityFramework.Tests.Models;

public class PropertyBuilderTestContext : DbContext
{
    private readonly ID1Generic client;

    public DbSet<EncryptedDataForPropertyBuilder> EncryptedData { get; set; } = null!;

    public PropertyBuilderTestContext(ID1Generic client, DbContextOptions options) : base(options)
    {
        this.client = client;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EncryptedDataForPropertyBuilder>().Property(x => x.Data!).IsConfidential(client);
        modelBuilder.Entity<EncryptedDataForPropertyBuilder>().Property(x => x.Binary!).IsConfidential(client);
    }
}

public class EncryptedDataForPropertyBuilder
{
    public int Id { get; set; }

    public string? Data { get; set; }

    public byte[]? Binary { get; set; }
}