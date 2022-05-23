// Copyright 2020-2022 CYBERCRYPT
using Encryptonize.Client;
using NSubstitute;
using NSubstitute.ClearExtensions;

namespace Encryptonize.EntityFramework.Tests.Utils;

// As EF models are processed only once, the mocked Encryptonize client needs to be the same
// for all tests, which is the reason for this slightly weird setup.
internal static class EncryptonizeClientMock
{
    private static readonly IEncryptonizeCore client = Substitute.For<IEncryptonizeCore>();

    public static IEncryptonizeCore Mock => client;

    public static void ClearSubstitute(ClearOptions options)
    {
        client.ClearSubstitute(options);
    }
}