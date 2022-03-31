using System.Text;

namespace Encryptonize.EntityFramework;

public static class ByteArrayExtensions
{
    public static string ToBase64(this string input)
    {
        return Convert.ToBase64String(input.GetBytes());
    }

    public static string ToBase64(this byte[] input)
    {
        return Convert.ToBase64String(input);
    }

    public static string FromBase64(this string input)
    {
        var bytes = Convert.FromBase64String(input);
        return Encoding.UTF8.GetString(bytes);
    }

    public static byte[] GetBytes(this string input)
    {
        return Encoding.UTF8.GetBytes(input);
    }

    public static string BytesToString(this byte[] input)
    {
        return Encoding.UTF8.GetString(input);
    }
}
