// Copyright 2020-2022 CYBERCRYPT
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("Encryptonize.EntityFramework.Tests")]

namespace Encryptonize.EntityFramework;

internal static class ByteArrayExtensions
{
    internal static string ToBase64(this string input)
    {
        return Convert.ToBase64String(input.GetBytes());
    }

    internal static string ToBase64(this byte[] input)
    {
        return Convert.ToBase64String(input);
    }

    internal static byte[] GetBytes(this string input)
    {
        return Encoding.UTF8.GetBytes(input);
    }

    internal static string BytesToString(this byte[] input)
    {
        return Encoding.UTF8.GetString(input);
    }
}
