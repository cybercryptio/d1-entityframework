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
            throw new NotSupportedException("Encryption columns if custom value converters is not supported");
        }

        return property.PropertyInfo?.GetCustomAttribute<EncryptedAttribute>(false) is not null;
    }
}
