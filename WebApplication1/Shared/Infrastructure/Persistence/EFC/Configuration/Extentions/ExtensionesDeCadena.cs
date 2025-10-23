using Humanizer;

namespace WebApplication1.Shared.Infrastructure.Persistence.EFC.Configuration.Extentions;
/// <summary>
///     Extensiones de Cadenas de Texto
/// </summary>
/// <remarks>
///     Esta clase contiene métodos de extensión para cadenas de texto.
///     Incluye métodos para convertir textos a snake_case y pluralizarlos.
/// </remarks>
public static class ExtensionesDeCadena
{
    /// <summary>
    ///     Convierte un texto a snake_case
    /// </summary>
    /// <param name="texto">El texto a convertir</param>
    /// <returns>El texto convertido a snake_case</returns>
    public static string ConvertirASnakeCase(this string texto)
    {
        return new string(Convertir(texto.GetEnumerator()).ToArray());

        // Método interno que hace la conversión carácter por carácter
        static IEnumerable<char> Convertir(CharEnumerator e)
        {
            if (!e.MoveNext()) yield break;

            // Primer carácter en minúscula
            yield return char.ToLower(e.Current);

            // Recorre el resto de caracteres
            while (e.MoveNext())
                if (char.IsUpper(e.Current))
                {
                    // Si encuentra una mayúscula, agrega '_' y la convierte a minúscula
                    yield return '_';
                    yield return char.ToLower(e.Current);
                }
                else
                {
                    // Mantiene el carácter tal cual
                    yield return e.Current;
                }
        }
    }
    /// <summary>
    ///     Pluraliza un texto
    /// </summary>
    /// <param name="texto">El texto a convertir</param>
    /// <returns>El texto convertido a plural</returns>
    public static string ConvertirAPlural(this string texto)
    {
        return texto.Pluralize(false);
    }
}