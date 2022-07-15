using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CyberCrypt.D1.EntityFramework;

internal static class PropertyEntryExtensions
{
    internal static Func<string?, IEnumerable<string>?>? FindSearchableKeywordsFunc(this PropertyEntry property)
    {
        var annotation = property.Metadata.FindAnnotation(Constants.SearchableKeywordsFuncAnnotationName);
        return annotation?.Value as Func<string?, IEnumerable<string>?>;
    }
}