// Copyright 2020-2022 CYBERCRYPT
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Encryptonize.EntityFramework;

public static class ModelExtensions
{
    public static bool ShouldEncrypt(this IMutableProperty property)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        if (property.FindAnnotation(CoreAnnotationNames.ValueConverter) is not null)
        {
            throw new NotSupportedException("Properties with custom value converters, cannot be marked as confidential");
        }

        return property.PropertyInfo?.GetCustomAttribute<ConfidentialAttribute>(false) is not null;
    }
}
