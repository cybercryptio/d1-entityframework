using Microsoft.EntityFrameworkCore.Metadata;

namespace CyberCrypt.D1.EntityFramework;

internal static class IPropertyExtensions
{
    internal static Func<string?, IEnumerable<string>?>? FindSearchableKeywordsFunc(this IProperty property)
    {
        var annotation = property.FindAnnotation(Constants.SearchableKeywordsFuncAnnotationName);
        return annotation?.Value as Func<string?, IEnumerable<string>?>;
    }
}