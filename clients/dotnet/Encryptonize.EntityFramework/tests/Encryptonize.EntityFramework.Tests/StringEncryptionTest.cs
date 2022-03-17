using System;
using System.Linq;
using Encryptonize.EntityFramework.Tests.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Encryptonize.EntityFramework.Tests;

public class StringEncryptionTest : IDisposable
{
    private readonly DbContextOptions<TestDbContext> contextOptions;
    private readonly SqliteConnection connection;

    public StringEncryptionTest()
    {
        connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        contextOptions = new DbContextOptionsBuilder<TestDbContext>().UseSqlite(connection).Options;
        using var context = new TestDbContext(contextOptions);
        context.Database.EnsureCreated();
        context.SaveChanges();
    }

    [Fact]
    public void SavingDataEncryptsData()
    {
        var expectedData = "test";
        using var dbContext = new TestDbContext(contextOptions);
        dbContext.EncryptedData.Add(new EncryptedData
        {
            Data = expectedData
        });
        dbContext.SaveChanges();

        var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT Data FROM EncryptedData";
        var result = command.ExecuteScalar() as string;
        Assert.Equal($"encrypted{expectedData.ToBase64()}", result);
    }


    [Fact]
    public void QueryingDataDecryptsData()
    {
        const string expectedData = "data";
        using var dbContext = new TestDbContext(contextOptions);
        var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = $"INSERT INTO EncryptedData (Data) VALUES ('encrypted{expectedData.ToBase64()}')";
        command.ExecuteNonQuery();

        var result = dbContext.EncryptedData.First();
        Assert.Equal(expectedData, result.Data);
    }

    public void Dispose() => connection.Dispose();
}