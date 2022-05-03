// Copyright 2020-2022 CYBERCRYPT
using Microsoft.EntityFrameworkCore;
using Encryptonize.Client;

namespace Encryptonize.EntityFramework.Tests.Models;

public class TestDbContext : DbContext
{
    private readonly IEncryptonizeClient client;

    public DbSet<EncryptedData> EncryptedData { get; set; } = null!;

    public TestDbContext(IEncryptonizeClient client, DbContextOptions options) : base(options)
    {
        this.client = client;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseEncryptonize(client);
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