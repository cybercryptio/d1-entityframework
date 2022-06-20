// Copyright 2020-2022 CYBERCRYPT
using CyberCrypt.D1.Client;
using NSubstitute;
using NSubstitute.ClearExtensions;

namespace CyberCrypt.D1.EntityFramework.Tests.Utils;

// As EF models are processed only once, the mocked Encryptonize client needs to be the same
// for all tests, which is the reason for this slightly weird setup.
internal static class D1ClientMock
{
    private static readonly ID1Generic client = Substitute.For<ID1Generic>();

    public static ID1Generic Mock => client;

    public static void ClearSubstitute(ClearOptions options)
    {
        client.ClearSubstitute(options);
    }
}