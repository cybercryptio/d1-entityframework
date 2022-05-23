// Copyright 2020-2022 CYBERCRYPT

using EncryptonizeDBSample.Models;
using Encryptonize.EntityFramework;
using Encryptonize.Client;
using Microsoft.EntityFrameworkCore;

namespace EncryptonizeDBSample.Data
{
    public class StorageContext : DbContext
    {
        private readonly IEncryptonizeCore client;

        public StorageContext(IEncryptonizeCore client, DbContextOptions<StorageContext> options) : base(options)
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