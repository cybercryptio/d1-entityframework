// Copyright 2020-2022 CYBERCRYPT

using EncryptonizeDBSample.Models;
using Encryptonize.EntityFramework;
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
            modelBuilder.UseEncryptonize(client);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Document> Documents => Set<Document>();
    }
}