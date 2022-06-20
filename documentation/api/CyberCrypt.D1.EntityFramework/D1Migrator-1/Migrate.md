# D1Migrator&lt;TContext&gt;.Migrate&lt;TEntry,TValue&gt; method

Migrate data from an unencrypted column to an encrypted column.

```csharp
public void Migrate<TEntry, TValue>(Func<TContext, IQueryable<TEntry>> where, 
    Func<TEntry, TValue> oldPropertyGetter, Action<TEntry, TValue> newPropertySetter, 
    int batchSize = 100)
```

| parameter | description |
| --- | --- |
| TEntry | The type of entry to migrate. |
| TValue | The type of value to migrate. |
| where | Filter to locate rows that should be migrated. |
| oldPropertyGetter | Func used to get the unecrypted value. |
| newPropertySetter | Action used to set the encrypted value. |
| batchSize | The number of rows to process at a time. |

## See Also

* class [D1Migrator&lt;TContext&gt;](../D1Migrator-1.md)
* namespace [CyberCrypt.D1.EntityFramework](../../CyberCrypt.D1.EntityFramework.md)

<!-- DO NOT EDIT: generated by xmldocmd for CyberCrypt.D1.EntityFramework.dll -->