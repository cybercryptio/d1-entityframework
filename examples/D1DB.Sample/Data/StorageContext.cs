// Copyright 2020-2022 CYBERCRYPT

using D1DB.Sample.Models;
using CyberCrypt.D1.EntityFramework;
using Microsoft.EntityFrameworkCore;
using CyberCrypt.D1.Client;

namespace D1DB.Sample.Data
{
    public class StorageContext : D1DbContext
    {
        public StorageContext(Func<ID1Generic> clientFactory, DbContextOptions<StorageContext> options)
            : base(clientFactory, options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>()
                .Property<string>("Data")
                .AddToSecureIndex(value => value?.Split(" "));
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Document> Documents => Set<Document>();
    }
}