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
        var alreadyMarkedAsEncrypting = encryptedAnnotation is not null && encryptedAnnotation.Value is bool && (bool)encryptedAnnotation.Value == true;
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
