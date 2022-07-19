// Copyright 2020-2022 CYBERCRYPT

using System;
using System.Collections.Generic;
using System.Linq;
using CyberCrypt.D1.Client;
using CyberCrypt.D1.Client.Response;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace CyberCrypt.D1.EntityFramework.Tests.Models;

public class SecureIndexTest
{
    private readonly DbContextOptions<SecureIndexTestPropertyBuilderTestContext> contextOptions;
    private readonly SqliteConnection connection;
    private readonly ID1Generic client = Substitute.For<ID1Generic>();

    public SecureIndexTest()
    {
        connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        contextOptions = new DbContextOptionsBuilder<SecureIndexTestPropertyBuilderTestContext>().UseSqlite(connection).Options;
        using var context = new SecureIndexTestPropertyBuilderTestContext(() => client, contextOptions);
        context.Database.EnsureCreated();
        context.SaveChanges();
    }

    [Fact]
    public void AddingDataAddsToIndex()
    {
        const string dataContent = "addData addTest";
        const string otherDataContent = "addOther1, addOther2";
        using var dbContext = new SecureIndexTestPropertyBuilderTestContext(() => client, contextOptions);
        dbContext.Data.Add(new PropertySearchableData
        {
            Data = dataContent,
            OtherData = otherDataContent,
        });
        dbContext.SaveChanges();

        var dataContentKeywords = dataContent.Split(" ");
        client.Index.Received(1).Add(
            Arg.Is<List<string>>(x => x.SequenceEqual(dataContentKeywords)),
            Arg.Is<string>("Data|Data|1"));
        var otherDataContentKeywords = otherDataContent.Split(", ");
        client.Index.Received(1).Add(
            Arg.Is<List<string>>(x => x.SequenceEqual(otherDataContentKeywords)),
            Arg.Is<string>("Data|OtherData|1"));
    }

    [Fact]
    public void DeletingDataRemovesFromIndex()
    {
        const string dataContent = "deleteData deleteTest";
        const string otherDataContent = "deleteOther1, deleteOther2";
        using var dbContext = new SecureIndexTestPropertyBuilderTestContext(() => client, contextOptions);
        var entry = new PropertySearchableData
        {
            Data = dataContent,
            OtherData = otherDataContent,
        };
        dbContext.Data.Add(entry);
        dbContext.SaveChanges();

        dbContext.Data.Remove(entry);
        dbContext.SaveChanges();

        var dataContentKeywords = dataContent.Split(" ");
        client.Index.Received(1).Delete(
            Arg.Is<List<string>>(x => x.SequenceEqual(dataContentKeywords)),
            Arg.Is<string>("Data|Data|1"));
        var otherDataContentKeywords = otherDataContent.Split(", ");
        client.Index.Received(1).Delete(
            Arg.Is<List<string>>(x => x.SequenceEqual(otherDataContentKeywords)),
            Arg.Is<string>("Data|OtherData|1"));
    }

    [Fact]
    public void UpdatingDataUpdatesTheIndex()
    {
        const string dataContent = "updateData updateTest";
        const string updateDataContent = "something else";
        const string otherDataContent = "updateOther1, updateOther2";
        const string updatedOtherDataContent = "updatedother";
        using var dbContext = new SecureIndexTestPropertyBuilderTestContext(() => client, contextOptions);
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

        var dataContentKeywords = dataContent.Split(" ");
        client.Index.Received(1).Delete(
            Arg.Is<List<string>>(x => x.SequenceEqual(dataContentKeywords)),
            Arg.Is<string>("Data|Data|1"));
        var otherDataContentKeywords = otherDataContent.Split(", ");
        client.Index.Received(1).Delete(
            Arg.Is<List<string>>(x => x.SequenceEqual(otherDataContentKeywords)),
            Arg.Is<string>("Data|OtherData|1"));
        var updatedDataContentKeywords = updateDataContent.Split(" ");
        client.Index.Received(1).Add(
            Arg.Is<List<string>>(x => x.SequenceEqual(updatedDataContentKeywords)),
            Arg.Is<string>("Data|Data|1"));
        var updatedOtherDataContentKeywords = updatedOtherDataContent.Split(", ");
        client.Index.Received(1).Add(
            Arg.Is<List<string>>(x => x.SequenceEqual(updatedOtherDataContentKeywords)),
            Arg.Is<string>("Data|OtherData|1"));
    }

    [Fact]
    public void ChangingNonSearchableDataDoesNotTouchTheIndex()
    {
        using var dbContext = new SecureIndexTestPropertyBuilderTestContext(() => client, contextOptions);
        var entry = new PropertySearchableData
        {
            NotSearchable = "anything",
        };
        dbContext.Data.Add(entry);
        dbContext.SaveChanges();

        entry.NotSearchable = "changed";
        dbContext.SaveChanges();

        client.Index.Received(0).Delete(
            Arg.Is<List<string>>(x => x.Contains("anything") || x.Contains("changed")),
            Arg.Is<string>("Data|Data|1"));
        client.Index.Received(0).Add(
            Arg.Is<List<string>>(x => x.Contains("anything") || x.Contains("changed")),
            Arg.Is<string>("Data|Data|1"));
    }

    [Fact]
    public void SavingChangedEntityWithoutChangingSearchablePropertyDoesNotTouchTheIndex()
    {
        using var dbContext = new SecureIndexTestPropertyBuilderTestContext(() => client, contextOptions);
        var entry = new PropertySearchableData
        {
            Data = "shouldBeUntouched",
            NotSearchable = "anything",
        };
        dbContext.Data.Add(entry);
        dbContext.SaveChanges();
        client.Index.ClearReceivedCalls();

        entry.NotSearchable = "changed";
        dbContext.SaveChanges();

        client.Index.Received(0).Add(
            Arg.Is<List<string>>(x => x.Contains("shouldBeUntouched")),
            Arg.Is<string>("Data|Data|1"));
        client.Index.Received(0).Delete(
            Arg.Is<List<string>>(x => x.Contains("shouldBeUntouched")),
            Arg.Is<string>("Data|Data|1"));
    }

    [Fact]
    public void SearchingForKeywordsOnNonSearchablePropertyThrows()
    {
        const string keyword = "noResult";
        using var dbContext = new SecureIndexTestPropertyBuilderTestContext(() => client, contextOptions);
        client.Index.Search(keyword).Returns(_ => new SearchResponse(new List<string>()));
        var entry = new PropertySearchableData();
        dbContext.Data.Add(entry);
        dbContext.SaveChanges();

        Assert.Throws<ArgumentException>(() => dbContext.Data.SecureIndexSearch(x => x.NotSearchable, keyword));
    }

    [Fact]
    public void SearchingForNonExistingKeywordWorks()
    {
        const string keyword = "noResult";
        using var dbContext = new SecureIndexTestPropertyBuilderTestContext(() => client, contextOptions);
        client.Index.Search(keyword).Returns(_ => new SearchResponse(new List<string>()));
        var entry = new PropertySearchableData();
        dbContext.Data.Add(entry);
        dbContext.SaveChanges();

        var result = dbContext.Data.SecureIndexSearch(x => x.Data, keyword);

        Assert.Empty(result);
    }

    [Fact]
    public void SearchingForExistingKeywordWorks()
    {
        const string keyword = "oneResult";
        using var dbContext = new SecureIndexTestPropertyBuilderTestContext(() => client, contextOptions);
        client.Index.Search(keyword).Returns(_ => new SearchResponse(new List<string>
            {
                "Data|Data|1"
            }));
        var entry = new PropertySearchableData
        {
            Data = keyword,
        };
        dbContext.Data.Add(entry);
        dbContext.SaveChanges();

        var result = dbContext.Data.SecureIndexSearch(x => x.Data, keyword).ToList();

        Assert.True(result.Count() == 1);
        Assert.Equal(entry.Id, result.First().Id);
        client.Index.Received(1).Search(keyword);
    }

    [Fact]
    public void SearchingForExistingMultipleKeywordsWorksByORingIds()
    {
        string[] keywords = new[] { "multipleKeywords1", "multipleKeywords2" };
        using var dbContext = new SecureIndexTestPropertyBuilderTestContext(() => client, contextOptions);
        client.Index.Search(keywords[0]).Returns(_ => new SearchResponse(new List<string>
            {
                "Data|Data|1",
                "Data|Data|2",
            }));
        client.Index.Search(keywords[1]).Returns(_ => new SearchResponse(new List<string>
            {
                "Data|Data|1"
            }));
        var firstEntry = new PropertySearchableData
        {
            Data = "multipleKeywords1 multipleKeywords2",
        };
        var secondEntry = new PropertySearchableData
        {
            Data = "multipleKeywords1",
        };
        dbContext.Data.Add(firstEntry);
        dbContext.Data.Add(secondEntry);
        dbContext.SaveChanges();

        var result = dbContext.Data.SecureIndexSearch(x => x.Data, keywords).ToList();

        Assert.True(result.Count() == 2);
        Assert.Equal(firstEntry.Id, result.First().Id);
        Assert.Equal(secondEntry.Id, result.Last().Id);
        client.Index.Received(1).Search(keywords[0]);
        client.Index.Received(1).Search(keywords[1]);
    }

    [Fact]
    public void SearchingWithMoreTwoMatchesWorks()
    {
        const string keyword = "twoMatches";
        using var dbContext = new SecureIndexTestPropertyBuilderTestContext(() => client, contextOptions);
        client.Index.Search(keyword).Returns(_ => new SearchResponse(new List<string>
            {
                "Data|Data|1",
                "Data|Data|2",
            }));
        var firstEntry = new PropertySearchableData
        {
            Data = "multipleKeywords1",
        };
        var secondEntry = new PropertySearchableData
        {
            Data = "multipleKeywords1",
        };
        dbContext.Data.Add(firstEntry);
        dbContext.Data.Add(secondEntry);
        dbContext.SaveChanges();

        var result = dbContext.Data.SecureIndexSearch(x => x.Data, keyword).ToList();

        Assert.True(result.Count() == 2);
        Assert.Equal(firstEntry.Id, result.First().Id);
        Assert.Equal(secondEntry.Id, result.Last().Id);
        client.Index.Received(1).Search(keyword);
    }

    [Fact]
    public void KeywordsForOtherPropertiesAreNotReturned()
    {
        const string keyword = "resultForOtherProperty";
        using var dbContext = new SecureIndexTestPropertyBuilderTestContext(() => client, contextOptions);
        client.Index.Search(keyword).Returns(_ => new SearchResponse(new List<string>
            {
                "Data|OtherData|1",
            }));
        var entry = new PropertySearchableData
        {
            OtherData = keyword,
        };
        dbContext.Data.Add(entry);
        dbContext.SaveChanges();

        var result = dbContext.Data.SecureIndexSearch(x => x.Data, keyword).ToList();

        Assert.Empty(result);
    }

    [Fact]
    public void ChainingLinqMethodsWorksOnResults()
    {
        const string keyword = "chainingMethodsWorks";
        const string otherString = "otherString";
        using var dbContext = new SecureIndexTestPropertyBuilderTestContext(() => client, contextOptions);
        client.Index.Search(keyword).Returns(_ => new SearchResponse(new List<string>
            {
                "Data|Data|1"
            }));
        var entry = new PropertySearchableData
        {
            Data = keyword,
            NotSearchable = otherString,
        };
        dbContext.Data.Add(entry);
        dbContext.SaveChanges();

        var result = dbContext
            .Data
            .SecureIndexSearch(x => x.Data, keyword)
            .Where(x => x.NotSearchable == otherString)
            .ToList();
        var noResults = dbContext
            .Data
            .SecureIndexSearch(x => x.Data, keyword)
            .Where(x => x.NotSearchable == "notExisting")
            .ToList();

        client.Index.Received(2).Search(keyword);
        Assert.True(result.Count == 1);
        Assert.Equal(entry.Id, result.First().Id);
        Assert.Empty(noResults);
    }
}