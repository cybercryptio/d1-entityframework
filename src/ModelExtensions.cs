// Copyright 2020-2022 CYBERCRYPT
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CyberCrypt.D1.EntityFramework;

internal static class ModelExtensions
{
    internal static bool ShouldEncrypt(this IMutableProperty property)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        // Check if already encrypted. Happens with inheritance.
        var encryptedAnnotation = property.FindAnnotation(Constants.EncryptedAnnotationName);
        var alreadyMarkedAsEncrypting = encryptedAnnotation is not null && encryptedAnnotation.Value is not null && encryptedAnnotation.Value is bool && (bool)encryptedAnnotation.Value == true;
        if (alreadyMarkedAsEncrypting)
        {
            return false;
        }

#pragma warning disable EF1001 // Internal EF Core API usage.
        if (property.FindAnnotation(CoreAnnotationNames.ValueConverter) is not null)
#pragma warning restore EF1001 // Internal EF Core API usage.
        {
            throw new NotSupportedException("Properties with custom value converters, cannot be marked as confidential");
        }

        return property.PropertyInfo?.GetCustomAttribute<ConfidentialAttribute>(false) is not null;
    }
}
