// Copyright 2020-2022 CYBERCRYPT

using EncryptonizeDBSample.Models;
using CyberCrypt.D1.EntityFramework;
using Microsoft.EntityFrameworkCore;
using CyberCrypt.D1.Client;

namespace EncryptonizeDBSample.Data
{
    public class StorageContext : DbContext
    {
        private readonly ID1Generic client;

        public StorageContext(ID1Generic client, DbContextOptions<StorageContext> options) : base(options)
        {
            this.client = client;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseD1(client);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Document> Documents => Set<Document>();
    }
}