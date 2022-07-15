// Copyright 2020-2022 CYBERCRYPT

using System;
using System.Collections.Generic;
using System.Linq;
using CyberCrypt.D1.EntityFramework.Tests.Utils;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace CyberCrypt.D1.EntityFramework.Tests.Models;

public class SearchableTest
{
    private readonly DbContextOptions<SearchablePropertyBuilderTestContext> contextOptions;
    private readonly SqliteConnection connection;

    public SearchableTest()
    {
        // Because the model is cache globally the mock needs to have all state cleared between each test
        D1ClientMock.ClearSubstitute(ClearOptions.All);

        connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        contextOptions = new DbContextOptionsBuilder<SearchablePropertyBuilderTestContext>().UseSqlite(connection).Options;
        using var context = new SearchablePropertyBuilderTestContext(() => D1ClientMock.Mock, contextOptions);
        context.Database.EnsureCreated();
        context.SaveChanges();
    }

    [Fact]
    public void AddingDataAddsToIndex()
    {
        const string dataContent = "addData addTest";
        const string otherDataContent = "addOther1, addOther2";
        using var dbContext = new SearchablePropertyBuilderTestContext(() => D1ClientMock.Mock, contextOptions);
        dbContext.Data.Add(new PropertySearchableData
        {
            Data = dataContent,
            OtherData = otherDataContent,
        });
        dbContext.SaveChanges();

        D1ClientMock.Mock.Searchable.Received(1).Add(Arg.Is<List<string>>(x => x.All(y => dataContent.Split(" ", StringSplitOptions.None).Contains(y))), Arg.Is<string>("Data|Data|1"));
        D1ClientMock.Mock.Searchable.Received(1).Add(Arg.Is<List<string>>(x => x.All(y => otherDataContent.Split(", ", StringSplitOptions.None).Contains(y))), Arg.Is<string>("Data|OtherData|1"));
    }

    [Fact]
    public void DeletingDataRemovesFromIndex()
    {
        const string dataContent = "deleteData deleteTest";
        const string otherDataContent = "deleteOther1, deleteOther2";
        using var dbContext = new SearchablePropertyBuilderTestContext(() => D1ClientMock.Mock, contextOptions);
        var entry = new PropertySearchableData
        {
            Data = dataContent,
            OtherData = otherDataContent,
        };
        dbContext.Data.Add(entry);
        dbContext.SaveChanges();

        dbContext.Data.Remove(entry);
        dbContext.SaveChanges();

        D1ClientMock.Mock.Searchable.Received(1).Delete(Arg.Is<List<string>>(x => x.All(y => dataContent.Split(" ", StringSplitOptions.None).Contains(y))), Arg.Is<string>("Data|Data|1"));
        D1ClientMock.Mock.Searchable.Received(1).Delete(Arg.Is<List<string>>(x => x.All(y => otherDataContent.Split(", ", StringSplitOptions.None).Contains(y))), Arg.Is<string>("Data|OtherData|1"));
    }

    [Fact]
    public void UpdatingDataUpdatesTheIndex()
    {
        const string dataContent = "updateData updateTest";
        const string updateDataContent = "something else";
        const string otherDataContent = "updateOther1, updateOther2";
        const string updatedOtherDataContent = "updatedother";
        using var dbContext = new SearchablePropertyBuilderTestContext(() => D1ClientMock.Mock, contextOptions);
        var entry = new PropertySearchableData
        {
            Data = dataContent,
            OtherData = otherDataContent,
        };
        dbContext.Data.Add(entry);
        dbContext.SaveChanges();

        entry.Data = updateDataContent;
        entry.OtherData = updatedOtherDataContent;
        dbContext.SaveChanges();

        D1ClientMock.Mock.Searchable.Received(1).Delete(Arg.Is<List<string>>(x => x.All(y => dataContent.Split(" ", StringSplitOptions.None).Contains(y))), Arg.Is<string>("Data|Data|1"));
        D1ClientMock.Mock.Searchable.Received(1).Delete(Arg.Is<List<string>>(x => x.All(y => otherDataContent.Split(", ", StringSplitOptions.None).Contains(y))), Arg.Is<string>("Data|OtherData|1"));
        D1ClientMock.Mock.Searchable.Received(1).Add(Arg.Is<List<string>>(x => x.All(y => updateDataContent.Split(" ", StringSplitOptions.None).Contains(y))), Arg.Is<string>("Data|Data|1"));
        D1ClientMock.Mock.Searchable.Received(1).Add(Arg.Is<List<string>>(x => x.All(y => updatedOtherDataContent.Split(", ", StringSplitOptions.None).Contains(y))), Arg.Is<string>("Data|OtherData|1"));
    }

    [Fact]
    public void ChangingNonSearchableDataDoesNotTouchTheIndex()
    {
        using var dbContext = new SearchablePropertyBuilderTestContext(() => D1ClientMock.Mock, contextOptions);
        var entry = new PropertySearchableData
        {
            NotSearchable = "anything",
        };
        dbContext.Data.Add(entry);
        dbContext.SaveChanges();

        entry.NotSearchable = "changed";
        dbContext.SaveChanges();

        D1ClientMock.Mock.Searchable.Received(0).Delete(Arg.Is<List<string>>(x => x.Contains("anything") || x.Contains("changed")), Arg.Is<string>("Data|Data|1"));
        D1ClientMock.Mock.Searchable.Received(0).Add(Arg.Is<List<string>>(x => x.Contains("anything") || x.Contains("changed")), Arg.Is<string>("Data|Data|1"));
    }

    [Fact]
    public void SavingChangedEntityWithoutChangingSearchablePropertyDoesNotTouchTheIndex()
    {
        using var dbContext = new SearchablePropertyBuilderTestContext(() => D1ClientMock.Mock, contextOptions);
        var entry = new PropertySearchableData
        {
            Data = "shouldBeUntouched",
            NotSearchable = "anything",
        };
        dbContext.Data.Add(entry);
        dbContext.SaveChanges();

        entry.NotSearchable = "changed";
        dbContext.SaveChanges();

        // The entry will be added once to the index, but not again when the NotSearchable property is changed.
        D1ClientMock.Mock.Searchable.Received(1).Add(Arg.Is<List<string>>(x => x.Contains("shouldBeUntouched")), Arg.Is<string>("Data|Data|1"));
        D1ClientMock.Mock.Searchable.Received(0).Delete(Arg.Is<List<string>>(x => x.Contains("shouldBeUntouched")), Arg.Is<string>("Data|Data|1"));
    }
}