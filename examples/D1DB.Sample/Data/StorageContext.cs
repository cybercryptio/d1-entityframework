// Copyright 2020-2022 CYBERCRYPT

using D1DB.Sample.Models;
using CyberCrypt.D1.EntityFramework;
using Microsoft.EntityFrameworkCore;
using CyberCrypt.D1.Client;

namespace D1DB.Sample.Data
{
    public class StorageContext : DbContext
    {
        private readonly Func<ID1Generic> clientFactory;

        public StorageContext(Func<ID1Generic> clientFactory, DbContextOptions<StorageContext> options)
            : base(options)
        {
            this.clientFactory = clientFactory;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseD1(clientFactory);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Document> Documents => Set<Document>();
    }
}