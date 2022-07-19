using CyberCrypt.D1.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CyberCrypt.D1.EntityFramework;

/// <summary>
/// D1 database context.
/// </summary>
public class D1DbContext : DbContext
{
    private readonly Func<ID1Generic> clientFactory;

    /// <summary>
    /// Gets the factory function used to create a new <see cref="ID1Generic"/> client.
    /// </summary>
    public Func<ID1Generic> ClientFactory => clientFactory;

    /// <summary>
    /// Create a new instance of <see cref="D1DbContext"/>.
    /// </summary>
    /// <param name="clientFactory">The factory for creating <see cref="ID1Generic"/> instances.</param>
    /// <param name="options">The options for the database context.</param>
    /// <returns>A new instance of <see cref="D1DbContext"/>.</returns>
    public D1DbContext(Func<ID1Generic> clientFactory, DbContextOptions options) : base(options)
    {
        this.clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
    }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseD1(clientFactory);
        base.OnModelCreating(modelBuilder);
    }

    /// <inheritdoc/>
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        var searchableEntries = FindSearchableEntries();
        var result = base.SaveChanges(acceptAllChangesOnSuccess);
        UpdateSearchIndex(clientFactory(), searchableEntries);
        return result;
    }

    /// <inheritdoc/>
    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        var searchableEntries = FindSearchableEntries();
        var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        UpdateSearchIndex(clientFactory(), searchableEntries);
        return result;
    }

    private IEnumerable<EntryMapping> FindSearchableEntries()
    {
        var mappings = new List<EntryMapping>();
        foreach (var entry in ChangeTracker.Entries().Where(x => x.State == EntityState.Deleted || x.State == EntityState.Modified || x.State == EntityState.Added))
        {
            var entity = entry.Entity;
            var primaryKey = entry.Metadata.FindPrimaryKey();
            var searchableProperties = new List<SearchableProperty>();
            foreach (var property in entry.Properties)
            {
                var keywordsFunc = property.FindSearchableKeywordsFunc();
                if (keywordsFunc is null)
                {
                    continue;
                }

                if (entry.State == EntityState.Modified && !entry.Property(property.Metadata.Name).IsModified)
                {
                    continue;
                }

                var oldKeywords = keywordsFunc(entry.OriginalValues[property.Metadata.Name]?.ToString());
                var newKeywords = keywordsFunc(entry.CurrentValues[property.Metadata.Name]?.ToString());
                searchableProperties.Add(new SearchableProperty(property, oldKeywords, newKeywords));
            }

            if (!searchableProperties.Any())
            {
                continue;
            }

            if (primaryKey is null)
            {
                throw new InvalidOperationException("Primary key not found.");
            }

            var primaryKeyProperty = primaryKey.Properties.SingleOrDefault();
            if (primaryKeyProperty is null)
            {
                throw new InvalidOperationException("Primary key must have exactly one property.");
            }

            mappings.Add(new EntryMapping(entry, entry.State, primaryKeyProperty, searchableProperties));
        }

        return mappings;
    }

    private void UpdateSearchIndex(ID1Generic client, IEnumerable<EntryMapping> mappings)
    {
        foreach (var mapping in mappings)
        {
            switch (mapping.EntryState)
            {
                case EntityState.Added:
                    ProcessAdded(client, mapping);
                    break;
                case EntityState.Modified:
                    ProcessModified(client, mapping);
                    break;
                case EntityState.Deleted:
                    ProcessDeleted(client, mapping);
                    break;
            }
        }
    }

    private void ProcessDeleted(ID1Generic client, EntryMapping mapping)
    {
        foreach (var property in mapping.SearchableProperties)
        {
            if (property.OldKeywords is null || !property.OldKeywords.Any())
            {
                continue;
            }

            var identifier = mapping.GetIdentifier(Model, property);
            client.Index.Delete(property.OldKeywords.ToList(), identifier);
        }
    }

    private void ProcessModified(ID1Generic client, EntryMapping mapping)
    {
        foreach (var property in mapping.SearchableProperties)
        {
            var identifier = mapping.GetIdentifier(Model, property);
            if (property.OldKeywords is not null && property.OldKeywords.Any())
            {
                client.Index.Delete(property.OldKeywords.ToList(), identifier);
            }

            if (property.NewKeywords is not null && property.NewKeywords.Any())
            {
                client.Index.Add(property.NewKeywords.ToList(), identifier);
            }
        }
    }

    private void ProcessAdded(ID1Generic client, EntryMapping mapping)
    {
        foreach (var property in mapping.SearchableProperties)
        {
            if (property.NewKeywords is null || !property.NewKeywords.Any())
            {
                continue;
            }

            var identifier = mapping.GetIdentifier(Model, property);
            client.Index.Add(property.NewKeywords.ToList(), identifier);
        }
    }

    private class EntryMapping
    {
        public EntityEntry Entry { get; set; }
        public EntityState EntryState { get; set; }
        public IProperty PrimaryKey { get; set; }
        public IEnumerable<SearchableProperty> SearchableProperties { get; set; }

        public EntryMapping(EntityEntry entry, EntityState entryState, IProperty primaryKey, IEnumerable<SearchableProperty> searchableProperties)
        {
            Entry = entry;
            EntryState = entry.State;
            PrimaryKey = primaryKey;
            SearchableProperties = searchableProperties;
        }

        public string GetIdentifier(IModel model, SearchableProperty property)
        {
            var entityType = model.FindEntityType(Entry.Entity.GetType())!;
            var tableName = entityType.GetSchemaQualifiedTableName() ?? "";
            var storeObjectIdentifier = StoreObjectIdentifier.Table(entityType.GetSchemaQualifiedTableName()!);
            var columnName = entityType.FindProperty(property.PropertyEntry.Metadata.Name)?.GetColumnName(storeObjectIdentifier) ?? "";
            var primaryKey = PrimaryKey.FieldInfo?.GetValue(Entry.Entity)?.ToString() ?? "";
            return $"{tableName}{Constants.IdentifierSeperator}{columnName}{Constants.IdentifierSeperator}{primaryKey}";
        }
    }

    private class SearchableProperty
    {
        public PropertyEntry PropertyEntry { get; set; }
        public IEnumerable<string>? OldKeywords { get; set; }
        public IEnumerable<string>? NewKeywords { get; set; }

        public SearchableProperty(PropertyEntry propertyEntry, IEnumerable<string>? oldKeywords, IEnumerable<string>? newKeywords)
        {
            PropertyEntry = propertyEntry;
            OldKeywords = oldKeywords;
            NewKeywords = newKeywords;
        }
    }
}