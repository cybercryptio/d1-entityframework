
// Copyright 2020-2022 CYBERCRYPT
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Encryptonize.Client;

namespace Encryptonize.EntityFramework;

public static class ModelBuilderExtensions
{
    private const int UUID_LENGHT = 36;
    private static readonly byte[] emptyByteArray = new byte[0];

    public static ModelBuilder UseEncryptonize(this ModelBuilder modelBuilder, IEncryptonizeClient client)
    {
        if (modelBuilder is null)
        {
            throw new ArgumentNullException(nameof(modelBuilder));
        }

        var stringConverter = new ValueConverter<string, string>(v => StringEncryptor(v, client), v => StringDecryptor(v, client));
        var binaryConverter = new ValueConverter<byte[], byte[]>(v => BinaryEncryptor(v, client), v => BinaryDecryptor(v, client));

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

    private static string StringEncryptor(string value, IEncryptonizeClient client)
    {
        var encryptedValue = BinaryEncryptor(value.GetBytes(), client);
        return encryptedValue.ToBase64();
    }

    private static string StringDecryptor(string value, IEncryptonizeClient client)
    {
        var bytesValue = Convert.FromBase64String(value);
        var plaintext = BinaryDecryptor(bytesValue, client);
        return plaintext.BytesToString();
    }

    private static byte[] BinaryEncryptor(byte[] value, IEncryptonizeClient client)
    {
        var res = client.Encrypt(value, emptyByteArray);
        var encryptedValue = res.ObjectId.GetBytes().Concat(res.Ciphertext).ToArray();
        return encryptedValue;
    }

    private static byte[] BinaryDecryptor(byte[] value, IEncryptonizeClient client)
    {
        var objectId = value.Take(UUID_LENGHT).ToArray().BytesToString();
        var ciphertext = value.Skip(UUID_LENGHT).ToArray();
        var res = client.Decrypt(objectId, ciphertext, emptyByteArray);
        return res.Plaintext;
    }
}