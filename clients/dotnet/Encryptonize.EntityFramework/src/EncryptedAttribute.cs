﻿namespace Encryptonize.EntityFramework;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class EncryptedAttribute : Attribute
{
}
