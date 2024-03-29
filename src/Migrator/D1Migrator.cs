// Copyright 2022 CYBERCRYPT
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// 	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using CyberCrypt.D1.Client;
using Microsoft.EntityFrameworkCore;

namespace CyberCrypt.D1.EntityFramework.Migrator;

/// <summary>
/// Migrator used to migrate unencrypted data to encrypted data.
/// </summary>
/// <typeparam name="TContext">The <see cref="DbContext" /> migration is done on.</typeparam>
public class D1Migrator<TContext> where TContext : DbContext
{
    private readonly TContext context;
    private readonly ID1Generic client;

    /// <summary>
    /// Initializes a new instance of the <see cref="D1Migrator{TContext}"/> class.
    /// </summary>
    /// <param name="context">The <see cref="DbContext" /> migration is done on.</param>
    /// <param name="client">The <see cref="ID1Generic" /> used to encrypt data.</param>
    public D1Migrator(TContext context, ID1Generic client)
    {
        this.context = context;
        this.client = client;
    }

    /// <summary>
    /// Migrate data from an unencrypted column to an encrypted column.
    /// </summary>
    /// <param name="where">Filter to locate rows that should be migrated.</param>
    /// <param name="oldPropertyGetter"><see cref="Func{T, TResult}" /> used to get the unecrypted value.</param>
    /// <param name="newPropertySetter"><see cref="Action{T, TResult}" /> used to set the encrypted value.</param>
    /// <param name="batchSize">The number of rows to process at a time.</param>
    /// <typeparam name="TEntry">The type of entry to migrate.</typeparam>
    /// <typeparam name="TValue">The type of value to migrate.</typeparam>
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
                var oldProp = oldPropertyGetter(entry)!;
                newPropertySetter(entry, oldProp);
                processed++;
            }

            context.SaveChanges();
        }
    }

    /// <inheritdoc cref="Migrate" />
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
                var oldProp = oldPropertyGetter(entry)!;
                newPropertySetter(entry, oldProp);
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
}