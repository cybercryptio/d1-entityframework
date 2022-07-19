using System.Linq.Expressions;
using System.Reflection;
using CyberCrypt.D1.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CyberCrypt.D1.Client;

/// <summary>
/// Extension methods for querying searchable keywords.
/// </summary>
public static class SearchableQueryExtensions
{
    /// <summary>
    /// Find all entity matching the provided keywords.
    /// </summary>
    public static IEnumerable<T> ConfidentialSearch<T, U>(this DbSet<T> dbSet,
        Expression<Func<T, U>> propertyAccessor,
        params string[] keywords) where T : class
    {
        var expr = ((LambdaExpression)propertyAccessor).Body;
        if (expr.NodeType != ExpressionType.MemberAccess)
        {
            throw new ArgumentException("Expression must be a member access expression");
        }
        var memberExpr = (MemberExpression)expr;
        if (memberExpr.Member.MemberType != MemberTypes.Property)
        {
            throw new ArgumentException("Expression must be a property access expression");
        }
        var propertyName = memberExpr.Member.Name;

#pragma warning disable EF1001 // Internal EF Core API usage.
        var efCoreInternalDbSet = (InternalDbSet<T>)dbSet;
#pragma warning restore EF1001 // Internal EF Core API usage.
        var dbContextInfo = efCoreInternalDbSet.GetType().GetField("_context", BindingFlags.Instance | BindingFlags.NonPublic);
        if (dbContextInfo is null)
        {
            throw new ArgumentException("Could not find DbContext on DbSet");
        }

        var dbContext = dbContextInfo.GetValue(efCoreInternalDbSet) as DbContext;
        if (dbContext is null)
        {
            throw new ArgumentException("Could get DbContext from DbSet");
        }

        var d1DbContext = dbContext as D1DbContext;
        if (d1DbContext is null)
        {
            throw new ArgumentException("DbContext must be a D1DbContext");
        }

#pragma warning disable EF1001 // Internal EF Core API usage.
        var entityType = efCoreInternalDbSet.EntityType;
#pragma warning restore EF1001 // Internal EF Core API usage.
        var entityProperty = entityType.FindProperty(propertyName);
        if (entityProperty is null)
        {
            throw new ArgumentException("Could not find property on entity type");
        }

        if (entityProperty.FindAnnotation(Constants.SearchableKeywordsFuncAnnotationName) is null)
        {
            throw new ArgumentException("Property must be searchable");
        }

        var primaryKey = entityType.FindPrimaryKey();
        if (primaryKey is null)
        {
            throw new ArgumentException("Entity must have a primary key");
        }

        var primaryKeyName = primaryKey.Properties.SingleOrDefault()?.Name;
        if (primaryKeyName is null)
        {
            throw new ArgumentException("Composite primary keys are not supported");
        }
        var primaryKeyFieldInfo = primaryKey.Properties.First().FieldInfo;
        var primaryKeyInfo = primaryKey.Properties.First().PropertyInfo;
        var client = d1DbContext.ClientFactory();

        var searchResults = keywords.SelectMany(x =>
        {
            return client.Searchable
                .Search(x)
                .Identifiers
                .Select(x =>
                {
                    var splitIdents = x.Split("|");
                    if (splitIdents.Length != 3)
                    {
                        throw new InvalidOperationException($"Received unsupported identifer format from D1: '{string.Join('|', splitIdents)}'");
                    }

                    var tableName = splitIdents[0];
                    var columnName = splitIdents[1];
                    var id = splitIdents[2];
                    return (tableName, columnName, id);
                });
        }
        ).ToList();

        if (!searchResults.Any())
        {
            return Enumerable.Empty<T>();
        }

        var entityTableName = entityType.GetSchemaQualifiedTableName();
        var storeObjectIdentifier = StoreObjectIdentifier.Table(entityType.GetSchemaQualifiedTableName()!);
        var entityColumnName = entityType.FindProperty(propertyName)?.GetColumnName(storeObjectIdentifier);
        var parameter = Expression.Parameter(typeof(T), "x");
        var idExpressions = new List<Expression>();
        foreach (var (tableName, columnName, id) in searchResults)
        {
            if (tableName != entityTableName || columnName != entityColumnName)
            {
                continue;
            }

            var castedId = Convert.ChangeType(id, primaryKey.GetKeyType());
            var idConstant = Expression.Constant(castedId, primaryKey.GetKeyType());
            var primaryKeyAccessExpr = Expression.MakeMemberAccess(parameter, primaryKeyInfo!);
            var equalExpr = Expression.Equal(primaryKeyAccessExpr, idConstant);
            idExpressions.Add(equalExpr);
        }

        if (!idExpressions.Any()) {
            return Enumerable.Empty<T>();
        }

        if (idExpressions.Count == 1)
        {
            var lambdaExpr = Expression.Lambda<Func<T, bool>>(idExpressions[0], parameter);
            return dbSet.Where(lambdaExpr);
        }

        BinaryExpression predicateExpr;
        var left = idExpressions[0];
        var right = idExpressions[1];
        for (var i = 1; i < idExpressions.Count; i++)
        {
            left = Expression.Or(left, idExpressions[i]);
        }
        predicateExpr = (BinaryExpression)left;

        var whereExpr = Expression.Lambda<Func<T, bool>>(predicateExpr, parameter);
        return dbSet.Where(whereExpr);
    }
}