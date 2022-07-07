// Copyright 2020-2022 CYBERCRYPT
using System;
using System.Linq;
using System.Threading.Tasks;
using CyberCrypt.D1.EntityFramework.Migrator;
using CyberCrypt.D1.EntityFramework.Tests.Models;
using CyberCrypt.D1.EntityFramework.Tests.Utils;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace CyberCrypt.D1.EntityFramework.Tests;

public class MigrationTest : IDisposable
{
    private readonly DbContextOptions<MigrationTestContext> contextOptions;
    private readonly SqliteConnection connection;

    public MigrationTest()
    {
        // Because the model is cache globally the mock needs to have all state cleared between each test
        D1ClientMock.ClearSubstitute(ClearOptions.All);

        connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        contextOptions = new DbContextOptionsBuilder<MigrationTestContext>().UseSqlite(connection).Options;
        using var context = new MigrationTestContext(D1ClientMock.Mock, contextOptions);
        context.Database.EnsureCreated();
        context.SaveChanges();
    }

    [Fact]
    public void MigratingStringPropertiesTest()
    {
        using var dbContext = new MigrationTestContext(D1ClientMock.Mock, contextOptions);
        var firstObjectId = Guid.NewGuid().ToString();
        var firstData = "first";
        var firstCiphertext = "anything1".GetBytes();
        var secondObjectId = Guid.NewGuid().ToString();
        var secondData = "second";
        var secondCiphertext = "anything2".GetBytes();
        dbContext.Data.Add(new MigrationData { UnencryptedData = firstData });
        dbContext.Data.Add(new MigrationData { UnencryptedData = secondData });
        dbContext.SaveChanges();
        D1ClientMock.Mock.Generic.Encrypt(Arg.Is<byte[]>(x => x.SequenceEqual(firstData.GetBytes())), Arg.Any<byte[]>())
            .Returns(new CyberCrypt.D1.Client.Response.EncryptResponse(firstObjectId, firstCiphertext, new byte[0]));
        D1ClientMock.Mock.Generic.Encrypt(Arg.Is<byte[]>(x => x.SequenceEqual(secondData.GetBytes())), Arg.Any<byte[]>())
            .Returns(new CyberCrypt.D1.Client.Response.EncryptResponse(secondObjectId, secondCiphertext, new byte[0]));

        var migrator = new D1Migrator<MigrationTestContext>(dbContext, D1ClientMock.Mock);
        migrator.Migrate(x => x.Data.Where(x => x.EncryptedData == null), x => x.UnencryptedData, (x, v) => x.EncryptedData = v);

        var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT EncryptedData FROM Data";
        using var reader = command.ExecuteReader();
        reader.Read();
        reader.GetString(0).Should().Be(firstObjectId.GetBytes().Concat(firstCiphertext).ToArray().ToBase64());
        reader.Read();
        reader.GetString(0).Should().Be(secondObjectId.GetBytes().Concat(secondCiphertext).ToArray().ToBase64());
    }

    [Fact]
    public async Task MigratingAsyncStringPropertiesTest()
    {
        using var dbContext = new MigrationTestContext(D1ClientMock.Mock, contextOptions);
        var firstObjectId = Guid.NewGuid().ToString();
        var firstData = "firstAsync";
        var firstCiphertext = "anythingAsync1".GetBytes();
        var secondObjectId = Guid.NewGuid().ToString();
        var secondData = "secondAsync";
        var secondCiphertext = "anythingAsync2".GetBytes();
        dbContext.Data.Add(new MigrationData { UnencryptedData = firstData });
        dbContext.Data.Add(new MigrationData { UnencryptedData = secondData });
        dbContext.SaveChanges();
        D1ClientMock.Mock.Generic.Encrypt(Arg.Is<byte[]>(x => x.SequenceEqual(firstData.GetBytes())), Arg.Any<byte[]>())
            .Returns(new CyberCrypt.D1.Client.Response.EncryptResponse(firstObjectId, firstCiphertext, new byte[0]));
        D1ClientMock.Mock.Generic.Encrypt(Arg.Is<byte[]>(x => x.SequenceEqual(secondData.GetBytes())), Arg.Any<byte[]>())
            .Returns(new CyberCrypt.D1.Client.Response.EncryptResponse(secondObjectId, secondCiphertext, new byte[0]));

        var migrator = new D1Migrator<MigrationTestContext>(dbContext, D1ClientMock.Mock);
        await migrator.MigrateAsync(x => x.Data.Where(x => x.EncryptedData == null), x => x.UnencryptedData, (x, v) => x.EncryptedData = v);

        var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT EncryptedData FROM Data";
        using var reader = command.ExecuteReader();
        reader.Read();
        reader.GetString(0).Should().Be(firstObjectId.GetBytes().Concat(firstCiphertext).ToArray().ToBase64());
        reader.Read();
        reader.GetString(0).Should().Be(secondObjectId.GetBytes().Concat(secondCiphertext).ToArray().ToBase64());
    }

    [Fact]
    public void MigratingBinaryPropertiesTest()
    {
        using var dbContext = new MigrationTestContext(D1ClientMock.Mock, contextOptions);
        var firstObjectId = Guid.NewGuid().ToString();
        var firstData = "first".GetBytes();
        var firstCiphertext = "anything1".GetBytes();
        var secondObjectId = Guid.NewGuid().ToString();
        var secondData = "second".GetBytes();
        var secondCiphertext = "anything2".GetBytes();
        dbContext.Data.Add(new MigrationData { UnencryptedBinaryData = firstData });
        dbContext.Data.Add(new MigrationData { UnencryptedBinaryData = secondData });
        dbContext.SaveChanges();
        D1ClientMock.Mock.Generic.Encrypt(Arg.Is<byte[]>(x => x.SequenceEqual(firstData)), Arg.Any<byte[]>())
            .Returns(new CyberCrypt.D1.Client.Response.EncryptResponse(firstObjectId, firstCiphertext, new byte[0]));
        D1ClientMock.Mock.Generic.Encrypt(Arg.Is<byte[]>(x => x.SequenceEqual(secondData)), Arg.Any<byte[]>())
            .Returns(new CyberCrypt.D1.Client.Response.EncryptResponse(secondObjectId, secondCiphertext, new byte[0]));

        var migrator = new D1Migrator<MigrationTestContext>(dbContext, D1ClientMock.Mock);
        migrator.Migrate(x => x.Data.Where(x => x.EncryptedBinaryData == null), x => x.UnencryptedBinaryData, (x, v) => x.EncryptedBinaryData = v);

        var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT EncryptedBinaryData FROM Data";
        using var reader = command.ExecuteReader();
        reader.Read();
         ((byte[])reader.GetValue(0)).Should().Equal(firstObjectId.GetBytes().Concat(firstCiphertext).ToArray());
        reader.Read();
         ((byte[])reader.GetValue(0)).Should().Equal(secondObjectId.GetBytes().Concat(secondCiphertext).ToArray());
    }

    [Fact]
    public async Task MigratingAsyncBinaryPropertiesTest()
    {
        using var dbContext = new MigrationTestContext(D1ClientMock.Mock, contextOptions);
        var firstObjectId = Guid.NewGuid().ToString();
        var firstData = "firstAsync".GetBytes();
        var firstCiphertext = "anythingAsync1".GetBytes();
        var secondObjectId = Guid.NewGuid().ToString();
        var secondData = "secondAsync".GetBytes();
        var secondCiphertext = "anythingAsync2".GetBytes();
        dbContext.Data.Add(new MigrationData { UnencryptedBinaryData = firstData });
        dbContext.Data.Add(new MigrationData { UnencryptedBinaryData = secondData });
        dbContext.SaveChanges();
        D1ClientMock.Mock.Generic.Encrypt(Arg.Is<byte[]>(x => x.SequenceEqual(firstData)), Arg.Any<byte[]>())
            .Returns(new CyberCrypt.D1.Client.Response.EncryptResponse(firstObjectId, firstCiphertext, new byte[0]));
        D1ClientMock.Mock.Generic.Encrypt(Arg.Is<byte[]>(x => x.SequenceEqual(secondData)), Arg.Any<byte[]>())
            .Returns(new CyberCrypt.D1.Client.Response.EncryptResponse(secondObjectId, secondCiphertext, new byte[0]));

        var migrator = new D1Migrator<MigrationTestContext>(dbContext, D1ClientMock.Mock);
        await migrator.MigrateAsync(x => x.Data.Where(x => x.EncryptedBinaryData == null), x => x.UnencryptedBinaryData, (x, v) => x.EncryptedBinaryData = v);

        var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT EncryptedBinaryData FROM Data";
        using var reader = command.ExecuteReader();
        reader.Read();
        ((byte[])reader.GetValue(0)).Should().Equal(firstObjectId.GetBytes().Concat(firstCiphertext).ToArray());
        reader.Read();
        ((byte[])reader.GetValue(0)).Should().Equal(secondObjectId.GetBytes().Concat(secondCiphertext).ToArray());
    }

    public void Dispose() => connection.Dispose();
}