// Copyright 2020-2022 CYBERCRYPT

using Encryptonize.Client;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Encryptonize.EntityFramework;

public static class PropertyBuilderExtensions
{
    public static PropertyBuilder<string> IsConfidential(this PropertyBuilder<string> property, IEncryptonizeClient client)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        return property.HasConversion(ValueConverterFactory.CreateStringConverter(client));
    }

    public static PropertyBuilder<byte[]> IsConfidential(this PropertyBuilder<byte[]> property, IEncryptonizeClient client)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        return property.HasConversion(ValueConverterFactory.CreateBinaryConverter(client));
    }
}