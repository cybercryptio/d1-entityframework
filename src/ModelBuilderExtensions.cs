
// Copyright 2020-2022 CYBERCRYPT
using Microsoft.EntityFrameworkCore;
using CyberCrypt.D1.Client;

namespace Encryptonize.EntityFramework;

/// <summary>
/// Extension methods for <see cref="ModelBuilder"/>.
/// </summary>
public static class ModelBuilderExtensions
{
    private const int UUID_LENGHT = 36;
    private static readonly byte[] emptyByteArray = new byte[0];

    /// <summary>
    /// Enable Encryptonize support for the given <see cref="ModelBuilder"/>.
    /// </summary>
    /// <param name="modelBuilder">The <see cref="ModelBuilder"/>.</param>
    /// <param name="client">The Encryptonize Core client.</param>
    public static ModelBuilder UseEncryptonize(this ModelBuilder modelBuilder, ID1Generic client)
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