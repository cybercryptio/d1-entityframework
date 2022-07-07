// Copyright 2020-2022 CYBERCRYPT
using System;
using System.Data;
using System.Linq;
using CyberCrypt.D1.EntityFramework.Tests.Utils;
using CyberCrypt.D1.EntityFramework.Tests.Models;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace CyberCrypt.D1.EntityFramework.Tests;

public class BinaryEncryptionPropertyBuilderTest : IDisposable
{
    private readonly DbContextOptions<PropertyBuilderTestContext> contextOptions;
    private readonly SqliteConnection connection;

    public BinaryEncryptionPropertyBuilderTest()
    {
        // Because the model is cache globally the mock needs to have all state cleared between each test
        D1ClientMock.ClearSubstitute(ClearOptions.All);

        connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        contextOptions = new DbContextOptionsBuilder<PropertyBuilderTestContext>().UseSqlite(connection).Options;
        using var context = new PropertyBuilderTestContext(D1ClientMock.Mock, contextOptions);
        context.Database.EnsureCreated();
        context.SaveChanges();
    }

    [Fact]
    public void SavingDataEncryptsData()
    {
        var expectedData = "test".GetBytes();
        var objectId = Guid.NewGuid().ToString();
        var ciphertext = "dshajdhsadjlkjdsdsækliiouew".GetBytes();
        var expectedEncryptedData = objectId.GetBytes().Concat(ciphertext).ToArray();
        D1ClientMock.Mock.Generic.Encrypt(Arg.Is<byte[]>(x => x.SequenceEqual(expectedData)), Arg.Any<byte[]>())
            .Returns(new CyberCrypt.D1.Client.Response.EncryptResponse(objectId, ciphertext, new byte[0]));
        using var dbContext = new PropertyBuilderTestContext(D1ClientMock.Mock, contextOptions);
        dbContext.EncryptedData.Add(new EncryptedDataForPropertyBuilder
        {
            Binary = expectedData
        });
        dbContext.SaveChanges();

        var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT Binary FROM EncryptedData";
        var result = command.ExecuteScalar() as byte[];

        result.Should().Equal(expectedEncryptedData);
    }


    [Fact]
    public void QueryingDataDecryptsData()
    {
        var expectedData = "data".GetBytes();
        var objectId = Guid.NewGuid().ToString();
        var ciphertext = "dshajdhsadjlkjdsdsækliiouew".GetBytes();
        var encryptedData = objectId.GetBytes().Concat(ciphertext).ToArray();
        D1ClientMock.Mock.Generic.Decrypt(objectId, Arg.Is<byte[]>(x => x.SequenceEqual(ciphertext)), Arg.Any<byte[]>())
            .Returns(new CyberCrypt.D1.Client.Response.DecryptResponse(expectedData, new byte[0]));
        using var dbContext = new PropertyBuilderTestContext(D1ClientMock.Mock, contextOptions);
        var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "INSERT INTO EncryptedData (Binary) VALUES (@data)";
        var parameter = command.CreateParameter();
        parameter.DbType = DbType.Binary;
        parameter.Value = encryptedData;
        parameter.ParameterName = "@data";
        command.Parameters.Add(parameter);
        command.ExecuteNonQuery();

        var result = dbContext.EncryptedData.First();

        result.Binary.Should().Equal(expectedData);
    }

    public void Dispose() => connection.Dispose();
}