using System;
using System.Linq;
using Encryptonize.EntityFramework.Tests.Models;
using Encryptonize.EntityFramework.Tests.Utils;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace Encryptonize.EntityFramework.Tests;

public class StringEncryptionTest : IDisposable
{
    private readonly DbContextOptions<TestDbContext> contextOptions;
    private readonly SqliteConnection connection;

    public StringEncryptionTest()
    {
        // Because the model is cache globally the mock needs to have all state cleared between each test
        EncryptonizeClientMock.ClearSubstitute(ClearOptions.All);

        connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        contextOptions = new DbContextOptionsBuilder<TestDbContext>().UseSqlite(connection).Options;
        using var context = new TestDbContext(EncryptonizeClientMock.Mock, contextOptions);
        context.Database.EnsureCreated();
        context.SaveChanges();
    }

    [Fact]
    public void SavingDataEncryptsData()
    {
        var expectedData = "test";
        var objectId = Guid.NewGuid().ToString();
        var ciphertext = "dshajdhsadjlkjdsdsækliiouew".GetBytes();
        EncryptonizeClientMock.Mock.Encrypt(Arg.Is<byte[]>(x => x.SequenceEqual(expectedData.GetBytes())), Arg.Any<byte[]>())
            .Returns(new Client.Response.EncryptResponse(objectId, ciphertext, new byte[0]));
        using var dbContext = new TestDbContext(EncryptonizeClientMock.Mock, contextOptions);
        dbContext.EncryptedData.Add(new EncryptedData
        {
            Data = expectedData
        });
        dbContext.SaveChanges();

        var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT Data FROM EncryptedData";
        var result = command.ExecuteScalar() as string;
        var expectedResult = objectId.GetBytes().Concat(ciphertext).ToArray().ToBase64();
        Assert.Equal(expectedResult, result);
    }


    [Fact]
    public void QueryingDataDecryptsData()
    {
        const string expectedData = "data";
        var objectId = Guid.NewGuid().ToString();
        var ciphertext = "dshajdhsadjlkjdsdsækliiouew";
        EncryptonizeClientMock.Mock.Decrypt(objectId, Arg.Is<byte[]>(x => x.SequenceEqual(ciphertext.GetBytes())), Arg.Any<byte[]>())
            .Returns(new Client.Response.DecryptResponse(expectedData.GetBytes(), new byte[0]));
        var encryptedData = objectId.GetBytes().Concat(ciphertext.GetBytes()).ToArray().ToBase64();
        using var dbContext = new TestDbContext(EncryptonizeClientMock.Mock, contextOptions);
        var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = $"INSERT INTO EncryptedData (Data) VALUES ('{encryptedData}')";
        command.ExecuteNonQuery();

        var result = dbContext.EncryptedData.First();
        Assert.Equal(expectedData, result.Data);
    }

    public void Dispose() => connection.Dispose();
}