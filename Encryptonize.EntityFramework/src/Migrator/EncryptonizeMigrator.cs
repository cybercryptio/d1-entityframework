// Copyright 2020-2022 CYBERCRYPT

using System.Linq.Expressions;
using System.Text;
using Encryptonize.Client;
using Microsoft.EntityFrameworkCore;

namespace Encryptonize.EntityFramework.Migrator;

public class EncryptonizeMigrator<TContext> where TContext : DbContext
{
    private readonly TContext context;
    private readonly IEncryptonizeClient client;

    public EncryptonizeMigrator(TContext context, IEncryptonizeClient client)
    {
        this.context = context;
        this.client = client;
    }

    public void Migrate<TEntry, TValue>(
        Func<TContext, IQueryable<TEntry>> where,
        Func<TEntry, TValue> oldPropertyGetter,
        Action<TEntry, TValue> newPropertySetter,
        int batchSize = 100)
    {
        ValidateType<TValue>();

        var processed = 0;
        while (true)
        {
            var entries = context.FromExpression(() => where(context)).Skip(processed).Take(batchSize).ToArray();
            if (!entries.Any())
            {
                break;
            }

            foreach (var entry in entries)
            {
                MapProperty(entry, oldPropertyGetter, newPropertySetter);
                processed++;
            }

            context.SaveChanges();
        }
    }

    public async Task MigrateAsync<TEntry, TValue>(
        Func<TContext, IQueryable<TEntry>> where,
        Func<TEntry, TValue> oldPropertyGetter,
        Action<TEntry, TValue> newPropertySetter,
        int batchSize = 100,
        CancellationToken cancellationToken = default)
    {
        ValidateType<TValue>();

        var processed = 0;
        while (true)
        {
            var entries = context.FromExpression(() => where(context)).Skip(processed).Take(batchSize).AsAsyncEnumerable();
            var empty = true;
            await foreach (var entry in entries)
            {
                MapProperty(entry, oldPropertyGetter, newPropertySetter);
                empty = false;
                processed++;
            }

            if (empty)
            {
                break;
            }

            await context.SaveChangesAsync();
        }
    }

    private static void ValidateType<T>()
    {
        var propType = typeof(T);
        if (propType != typeof(string) && propType != typeof(byte[]))
        {
            throw new ArgumentException("Only string and byte[] properties are supported");
        }
    }

    private static void MapProperty<TEntry, TValue>(TEntry entry, Func<TEntry, TValue> oldPropertyGetter, Action<TEntry, TValue> newPropertySetter)
    {
        var oldProp = oldPropertyGetter(entry)!;
        newPropertySetter(entry, oldProp);
    }
}