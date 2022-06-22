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
    public static PropertyBuilder<string> IsConfidential(this PropertyBuilder<string> property, ID1Generic client)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        return property.HasConversion(ValueConverterFactory.CreateStringConverter(client));
    }

    /// <summary>
    /// Marks a property as confidential and to be encrypted using D1.
    /// </summary>
    public static PropertyBuilder<byte[]> IsConfidential(this PropertyBuilder<byte[]> property, ID1Generic client)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        return property.HasConversion(ValueConverterFactory.CreateBinaryConverter(client));
    }
}