using CyberCrypt.D1.Client;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CyberCrypt.D1.EntityFramework;

internal static class ValueConverterFactory
{
    private const int UUID_LENGHT = 36;
    private static readonly byte[] emptyByteArray = new byte[0];

    internal static ValueConverter<string, string> CreateStringConverter(Func<ID1Generic> clientFactory)
    {
        return new ValueConverter<string, string>(v => StringEncryptor(v, clientFactory), v => StringDecryptor(v, clientFactory));
    }

    internal static ValueConverter<byte[], byte[]> CreateBinaryConverter(Func<ID1Generic> clientFactory)
    {
        return new ValueConverter<byte[], byte[]>(v => BinaryEncryptor(v, clientFactory), v => BinaryDecryptor(v, clientFactory));
    }

    private static string StringEncryptor(string value, Func<ID1Generic> clientFactory)
    {
        var encryptedValue = BinaryEncryptor(value.GetBytes(), clientFactory);
        return encryptedValue.ToBase64();
    }

    private static string StringDecryptor(string value, Func<ID1Generic> clientFactory)
    {
        var bytesValue = Convert.FromBase64String(value);
        var plaintext = BinaryDecryptor(bytesValue, clientFactory);
        return plaintext.BytesToString();
    }

    private static byte[] BinaryEncryptor(byte[] value, Func<ID1Generic> clientFactory)
    {
        var res = clientFactory().Generic.Encrypt(value, emptyByteArray);
        var encryptedValue = res.ObjectId.GetBytes().Concat(res.Ciphertext).ToArray();
        return encryptedValue;
    }

    private static byte[] BinaryDecryptor(byte[] value, Func<ID1Generic> clientFactory)
    {
        var objectId = value.Take(UUID_LENGHT).ToArray().BytesToString();
        var ciphertext = value.Skip(UUID_LENGHT).ToArray();
        var res = clientFactory().Generic.Decrypt(objectId, ciphertext, emptyByteArray);
        return res.Plaintext;
    }
}