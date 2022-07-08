// Copyright 2020-2022 CYBERCRYPT
using Microsoft.EntityFrameworkCore;
using CyberCrypt.D1.Client;
using System;

namespace CyberCrypt.D1.EntityFramework.Tests.Models;

public class TestDbContext : DbContext
{
    private readonly Func<ID1Generic> clientFactory;

    public DbSet<EncryptedData> EncryptedData { get; set; } = null!;

    public TestDbContext(Func<ID1Generic> clientFactory, DbContextOptions options) : base(options)
    {
        this.clientFactory = clientFactory;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseD1(clientFactory);
        base.OnModelCreating(modelBuilder);
    }
}

public class EncryptedData
{
    public int Id { get; set; }

    [Confidential]
    public string? Data { get; set; }

    [Confidential]
    public byte[]? Binary { get; set; }
}