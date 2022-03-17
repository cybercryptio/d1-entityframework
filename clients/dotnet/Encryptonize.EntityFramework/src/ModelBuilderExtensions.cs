using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Encryptonize.EntityFramework;

public static class ModelBuilderExtensions
{
    // TODO: Once the Encryptonize client the conversions should be changed
    private static readonly ValueConverter StringConverter = new ValueConverter<string, string>(v => $"encrypted{v.ToBase64()}", v => v.Substring(9).FromBase64(), false);
    private static readonly ValueConverter BinaryConverter = new ValueConverter<byte[], byte[]>(v => "encrypted".GetBytes().Concat(v).ToArray(), v => v.Skip("encrypted".GetBytes().Length).ToArray(), false);

    public static ModelBuilder UseEncryptonize(this ModelBuilder modelBuilder)
    {
        if (modelBuilder is null)
        {
            throw new ArgumentNullException(nameof(modelBuilder));
        }

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties().Where(x => x.ShouldEncrypt()))
            {
                if (property.ClrType == typeof(string))
                {
                    property.SetValueConverter(StringConverter);
                }
                else if (property.ClrType == typeof(byte[]))
                {
                    property.SetValueConverter(BinaryConverter);
                }
                else
                {
                    throw new NotSupportedException($"Encryption column of type '{property.ClrType.FullName}' is not supported");
                }
            }
        }

        return modelBuilder;
    }
}