// Copyright 2022 CYBERCRYPT
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// 	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("CyberCrypt.D1.EntityFramework.Tests")]

namespace CyberCrypt.D1.EntityFramework;

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
