// Copyright 2020-2022 CYBERCRYPT
namespace Encryptonize.EntityFramework;

/// <summary>
/// Attribute used to mark data as confidential.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class ConfidentialAttribute : Attribute
{
}
