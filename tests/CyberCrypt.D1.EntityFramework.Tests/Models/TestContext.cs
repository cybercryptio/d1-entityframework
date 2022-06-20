// Copyright 2020-2022 CYBERCRYPT
using Microsoft.EntityFrameworkCore;
using CyberCrypt.D1.Client;

namespace CyberCrypt.D1.EntityFramework.Tests.Models;

public class TestDbContext : DbContext
{
    private readonly ID1Generic client;

    public DbSet<EncryptedData> EncryptedData { get; set; } = null!;

    public TestDbContext(ID1Generic client, DbContextOptions options) : base(options)
    {
        this.client = client;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseD1(client);
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