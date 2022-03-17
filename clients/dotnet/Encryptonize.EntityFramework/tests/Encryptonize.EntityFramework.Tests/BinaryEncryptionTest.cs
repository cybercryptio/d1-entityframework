using System;
using System.Data;
using System.Linq;
using System.Text;
using Encryptonize.EntityFramework.Tests.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Encryptonize.EntityFramework.Tests;

public class BinaryEncryptionTest : IDisposable
{
    private readonly DbContextOptions<TestDbContext> contextOptions;
    private readonly SqliteConnection connection;

    public BinaryEncryptionTest()
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
        var expectedData = Encoding.UTF8.GetBytes("test");
        var expectedEncryptedData = Encoding.UTF8.GetBytes("encrypted").Concat(expectedData).ToArray();
        using var dbContext = new TestDbContext(contextOptions);
        dbContext.EncryptedData.Add(new EncryptedData
        {
            Binary = expectedData
        });
        dbContext.SaveChanges();

        var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT Binary FROM EncryptedData";
        var result = command.ExecuteScalar() as byte[];
        Assert.Equal(expectedEncryptedData, result);
    }


    [Fact]
    public void QueryingDataDecryptsData()
    {
        var expectedData = Encoding.UTF8.GetBytes("data");
        var encryptedData = Encoding.UTF8.GetBytes("encrypted").Concat(expectedData).ToArray();
        using var dbContext = new TestDbContext(contextOptions);
        var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "INSERT INTO EncryptedData (Binary) VALUES (@data)";
        var parameter = command.CreateParameter();
        parameter.DbType = DbType.Binary;
        parameter.Value = encryptedData;
        parameter.ParameterName = "@data";
        command.Parameters.Add(parameter);
        command.ExecuteNonQuery();

        var result = dbContext.EncryptedData.First();
        Assert.Equal(expectedData, result.Binary);
    }

    public void Dispose() => connection.Dispose();
}