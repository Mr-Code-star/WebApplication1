using System.Text.RegularExpressions;

namespace WebApplication1.Shared.Infrastructure.Interfaces.ASP.Configuration.Extensions;

public static partial class ExtensionesDeCadena
{
    /// <summary>
    /// Convierte un texto a kebab-case
    /// </summary>
    /// <param name="texto">El texto a convertir</param>
    /// <returns>El texto convertido a kebab-case</returns>
    public static string ConvertirAKebabCase(this string texto)
    {
        if (string.IsNullOrEmpty(texto))
        {
            return texto;
        }

        return RegexKebabCase().Replace(texto, "-$1")
            .Trim()
            .ToLower();
    }

    [GeneratedRegex("(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", RegexOptions.Compiled)]
    private static partial Regex RegexKebabCase();
}