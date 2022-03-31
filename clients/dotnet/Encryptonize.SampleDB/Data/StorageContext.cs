// Copyright 2020-2022 CYBERCRYPT

using EncryptonizeDBSample.Models;
using Encryptonize.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace EncryptonizeDBSample.Data
{
    public class StorageContext : DbContext
    {
        public StorageContext(DbContextOptions<StorageContext> options) : base(options)
        {
        }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseEncryptonize();
        base.OnModelCreating(modelBuilder);
    }

        public DbSet<Document> Documents => Set<Document>();
    }
}