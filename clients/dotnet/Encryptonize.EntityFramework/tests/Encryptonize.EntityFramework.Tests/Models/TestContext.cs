using Microsoft.EntityFrameworkCore;

namespace Encryptonize.EntityFramework.Tests.Models;

public class TestDbContext : DbContext
{
    public DbSet<EncryptedData> EncryptedData { get; set; } = null!;

    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseEncryptonize();
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