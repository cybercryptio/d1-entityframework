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

using CyberCrypt.D1.Client;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CyberCrypt.D1.EntityFramework;

/// <summary>
/// Extentions for configuration data model properties
/// </summary>
public static class PropertyBuilderExtensions
{
    /// <summary>
    /// Marks a property as confidential and to be encrypted using D1.
    /// </summary>
    public static PropertyBuilder<string> IsConfidential(this PropertyBuilder<string> property, Func<ID1Generic> clientFactory)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        return property.HasConversion(ValueConverterFactory.CreateStringConverter(clientFactory));
    }

    /// <summary>
    /// Marks a property as confidential and to be encrypted using D1.
    /// </summary>
    public static PropertyBuilder<byte[]> IsConfidential(this PropertyBuilder<byte[]> property, Func<ID1Generic> clientFactory)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        return property.HasConversion(ValueConverterFactory.CreateBinaryConverter(clientFactory));
    }

    /// <summary>
    /// Marks a property as searchable.
    /// </summary>
    /// <param name="property">The property to mark as searchable.</param>
    /// <param name="keywordsFunc">Function calculation keywords for the property.</param>
    public static PropertyBuilder<string> AsSearchable(this PropertyBuilder<string> property, Func<string?, IEnumerable<string>?> keywordsFunc)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        return property.HasAnnotation(Constants.SearchableKeywordsFuncAnnotationName, keywordsFunc);
    }
}