// Copyright 2020-2022 CYBERCRYPT
namespace Encryptonize.EntityFramework;

/// <summary>
/// Attribute for marking a property as confidential and to be encrypted using Encryptonize.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class ConfidentialAttribute : Attribute
{
}
