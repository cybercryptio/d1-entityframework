// Copyright 2020-2022 CYBERCRYPT

using Encryptonize.Client;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Encryptonize.EntityFramework;

public static class PropertyBuilderExtensions
{
    /// <summary>
    /// Marks a property as confidential and to be encrypted using Encryptonize.
    /// </summary>
    public static PropertyBuilder<string> IsConfidential(this PropertyBuilder<string> property, IEncryptonizeClient client)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        return property.HasConversion(ValueConverterFactory.CreateStringConverter(client));
    }

    /// <summary>
    /// Marks a property as confidential and to be encrypted using Encryptonize.
    /// </summary>
    public static PropertyBuilder<byte[]> IsConfidential(this PropertyBuilder<byte[]> property, IEncryptonizeClient client)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        return property.HasConversion(ValueConverterFactory.CreateBinaryConverter(client));
    }
}