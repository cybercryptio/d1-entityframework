// Copyright 2020-2022 CYBERCRYPT

using CyberCrypt.D1.Client;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CyberCrypt.D1.EntityFramework;

/// <summary>
/// Extentions for configuration data model properties
/// </summary>
public static class PropertyBuilderExtensions
{
    /// <summary>
    /// Marks a property as confidential and to be encrypted using D1.
    /// </summary>
    public static PropertyBuilder<string> IsConfidential(this PropertyBuilder<string> property, Func<ID1Generic> clientFactory)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        return property.HasConversion(ValueConverterFactory.CreateStringConverter(clientFactory));
    }

    /// <summary>
    /// Marks a property as confidential and to be encrypted using D1.
    /// </summary>
    public static PropertyBuilder<byte[]> IsConfidential(this PropertyBuilder<byte[]> property, Func<ID1Generic> clientFactory)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        return property.HasConversion(ValueConverterFactory.CreateBinaryConverter(clientFactory));
    }

    /// <summary>
    /// Marks a property as added to the secure index.
    /// </summary>
    /// <param name="property">The property to mark as searchable.</param>
    /// <param name="keywordsFunc">Function calculation keywords for the property.</param>
    public static PropertyBuilder<string> AddToSecureIndex(this PropertyBuilder<string> property, Func<string?, IEnumerable<string>?> keywordsFunc)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        return property.HasAnnotation(Constants.SearchableKeywordsFuncAnnotationName, keywordsFunc);
    }
}