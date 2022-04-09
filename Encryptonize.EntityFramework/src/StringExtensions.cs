// Copyright 2020-2022 CYBERCRYPT
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

    public static byte[] GetBytes(this string input)
    {
        return Encoding.UTF8.GetBytes(input);
    }

    public static string BytesToString(this byte[] input)
    {
        return Encoding.UTF8.GetString(input);
    }
}
