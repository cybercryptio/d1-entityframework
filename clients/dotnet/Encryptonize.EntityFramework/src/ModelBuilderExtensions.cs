
// Copyright 2020-2022 CYBERCRYPT
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Encryptonize.Client;

namespace Encryptonize.EntityFramework;

public static class ModelBuilderExtensions
{
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
        var res = client.Encrypt(value.GetBytes(), emptyByteArray);
        var encryptedValue = res.ObjectId.GetBytes().Concat(res.Ciphertext).ToArray();
        return encryptedValue.ToBase64();
    }

    private static string StringDecryptor(string value, IEncryptonizeClient client)
    {
        var bytesValue = value.FromBase64().GetBytes();
        var objectId = bytesValue.Take(36).ToArray().BytesToString();
        var ciphertext = bytesValue.Skip(36).ToArray();
        var res = client.Decrypt(objectId, ciphertext, emptyByteArray);
        return res.Plaintext.BytesToString();
    }

    private static byte[] BinaryEncryptor(byte[] value, IEncryptonizeClient client)
    {
        var res = client.Encrypt(value, emptyByteArray);
        var encryptedValue = res.ObjectId.GetBytes().Concat(res.Ciphertext).ToArray();
        return encryptedValue;       
    }

    private static byte[] BinaryDecryptor(byte[] value, IEncryptonizeClient client)
    {
        var objectId = value.Take(36).ToArray().BytesToString();
        var ciphertext = value.Skip(36).ToArray();
        var res = client.Decrypt(objectId, ciphertext, emptyByteArray);
        return res.Plaintext;      
    }
}