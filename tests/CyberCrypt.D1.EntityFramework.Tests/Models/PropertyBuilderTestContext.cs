// Copyright 2020-2022 CYBERCRYPT
using Microsoft.EntityFrameworkCore;
using CyberCrypt.D1.Client;
using System;

namespace CyberCrypt.D1.EntityFramework.Tests.Models;

public class PropertyBuilderTestContext : DbContext
{
    private readonly Func<ID1Generic> clientFactory;

    public DbSet<EncryptedDataForPropertyBuilder> EncryptedData { get; set; } = null!;

    public PropertyBuilderTestContext(Func<ID1Generic> clientFactory, DbContextOptions options) : base(options)
    {
        this.clientFactory = clientFactory;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EncryptedDataForPropertyBuilder>().Property(x => x.Data!).IsConfidential(clientFactory);
        modelBuilder.Entity<EncryptedDataForPropertyBuilder>().Property(x => x.Binary!).IsConfidential(clientFactory);
    }
}

public class EncryptedDataForPropertyBuilder
{
    public int Id { get; set; }

    public string? Data { get; set; }

    public byte[]? Binary { get; set; }
}