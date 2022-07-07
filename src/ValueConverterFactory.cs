using CyberCrypt.D1.Client;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CyberCrypt.D1.EntityFramework;

internal static class ValueConverterFactory
{
    private const int UUID_LENGHT = 36;
    private static readonly byte[] emptyByteArray = new byte[0];

    internal static ValueConverter<string, string> CreateStringConverter(ID1Generic client)
    {
        return new ValueConverter<string, string>(v => StringEncryptor(v, client), v => StringDecryptor(v, client));
    }

    internal static ValueConverter<byte[], byte[]> CreateBinaryConverter(ID1Generic client)
    {
        return new ValueConverter<byte[], byte[]>(v => BinaryEncryptor(v, client), v => BinaryDecryptor(v, client));
    }

    private static string StringEncryptor(string value, ID1Generic client)
    {
        var encryptedValue = BinaryEncryptor(value.GetBytes(), client);
        return encryptedValue.ToBase64();
    }

    private static string StringDecryptor(string value, ID1Generic client)
    {
        var bytesValue = Convert.FromBase64String(value);
        var plaintext = BinaryDecryptor(bytesValue, client);
        return plaintext.BytesToString();
    }

    private static byte[] BinaryEncryptor(byte[] value, ID1Generic client)
    {
        var res = client.Generic.Encrypt(value, emptyByteArray);
        var encryptedValue = res.ObjectId.GetBytes().Concat(res.Ciphertext).ToArray();
        return encryptedValue;
    }

    private static byte[] BinaryDecryptor(byte[] value, ID1Generic client)
    {
        var objectId = value.Take(UUID_LENGHT).ToArray().BytesToString();
        var ciphertext = value.Skip(UUID_LENGHT).ToArray();
        var res = client.Generic.Decrypt(objectId, ciphertext, emptyByteArray);
        return res.Plaintext;
    }
}