using Encryptonize.Client;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Encryptonize.EntityFramework;

internal static class ValueConverterFactory
{
    private const int UUID_LENGHT = 36;
    private static readonly byte[] emptyByteArray = new byte[0];

    internal static ValueConverter<string, string> CreateStringConverter(IEncryptonizeCore client)
    {
        return new ValueConverter<string, string>(v => StringEncryptor(v, client), v => StringDecryptor(v, client));
    }

    internal static ValueConverter<byte[], byte[]> CreateBinaryConverter(IEncryptonizeCore client)
    {
        return new ValueConverter<byte[], byte[]>(v => BinaryEncryptor(v, client), v => BinaryDecryptor(v, client));
    }

    private static string StringEncryptor(string value, IEncryptonizeCore client)
    {
        var encryptedValue = BinaryEncryptor(value.GetBytes(), client);
        return encryptedValue.ToBase64();
    }

    private static string StringDecryptor(string value, IEncryptonizeCore client)
    {
        var bytesValue = Convert.FromBase64String(value);
        var plaintext = BinaryDecryptor(bytesValue, client);
        return plaintext.BytesToString();
    }

    private static byte[] BinaryEncryptor(byte[] value, IEncryptonizeCore client)
    {
        var res = client.Encrypt(value, emptyByteArray);
        var encryptedValue = res.ObjectId.GetBytes().Concat(res.Ciphertext).ToArray();
        return encryptedValue;
    }

    private static byte[] BinaryDecryptor(byte[] value, IEncryptonizeCore client)
    {
        var objectId = value.Take(UUID_LENGHT).ToArray().BytesToString();
        var ciphertext = value.Skip(UUID_LENGHT).ToArray();
        var res = client.Decrypt(objectId, ciphertext, emptyByteArray);
        return res.Plaintext;
    }
}