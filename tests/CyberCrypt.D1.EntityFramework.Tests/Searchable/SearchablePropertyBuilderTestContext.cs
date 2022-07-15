// Copyright 2020-2022 CYBERCRYPT
using Microsoft.EntityFrameworkCore;
using CyberCrypt.D1.Client;
using System;

namespace CyberCrypt.D1.EntityFramework.Tests.Models;

public class SearchablePropertyBuilderTestContext : D1DbContext
{
    private readonly Func<ID1Generic> clientFactory;

    public DbSet<PropertySearchableData> Data { get; set; } = null!;

    public SearchablePropertyBuilderTestContext(Func<ID1Generic> clientFactory, DbContextOptions options) : base(clientFactory, options)
    {
        this.clientFactory = clientFactory;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PropertySearchableData>().Property(x => x.Data!).AsSearchable(value => value?.Split(" "));
        modelBuilder.Entity<PropertySearchableData>().Property(x => x.OtherData!).AsSearchable(value => value?.Split(", "));
    }
}

public class PropertySearchableData
{
    public int Id { get; set; }

    public string? Data { get; set; }

    public string? OtherData { get; set; }

    public string? NotSearchable { get; set; }
}
