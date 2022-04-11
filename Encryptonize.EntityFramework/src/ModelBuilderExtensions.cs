
// Copyright 2020-2022 CYBERCRYPT
using Microsoft.EntityFrameworkCore;
using Encryptonize.Client;

namespace Encryptonize.EntityFramework;

public static class ModelBuilderExtensions
{
    /// <summary>
    /// Build the model with attribute based Encryptonize support.
    /// </summary>
    public static ModelBuilder UseEncryptonize(this ModelBuilder modelBuilder, IEncryptonizeClient client)
    {
        if (modelBuilder is null)
        {
            throw new ArgumentNullException(nameof(modelBuilder));
        }

        var stringConverter = ValueConverterFactory.CreateStringConverter(client);
        var binaryConverter = ValueConverterFactory.CreateBinaryConverter(client);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties().Where(x => x.ShouldEncrypt()))
            {
                if (property.ClrType == typeof(string))
                {
                    property.SetValueConverter(stringConverter);
                }
                else if (property.ClrType == typeof(byte[]))
                {
                    property.SetValueConverter(binaryConverter);
                }
                else
                {
                    throw new NotSupportedException($"Encryption column of type '{property.ClrType.FullName}' is not supported");
                }
            }
        }

        return modelBuilder;
    }
}