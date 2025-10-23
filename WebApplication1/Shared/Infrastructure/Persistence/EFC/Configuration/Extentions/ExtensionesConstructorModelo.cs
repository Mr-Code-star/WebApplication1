using Humanizer;
using Microsoft.EntityFrameworkCore;
namespace WebApplication1.Shared.Infrastructure.Persistence.EFC.Configuration.Extentions;
/// <summary>
///     Extensiones del Constructor de Modelos
/// </summary>
/// <remarks>
///     Esta clase contiene métodos de extensión para el constructor de modelos.
///     Incluye un método para usar la convención de nombres snake_case y pluralizar los nombres de tablas.
/// </remarks>
/// 
public static class ExtensionesConstructorModelo
{
    /// <summary>
    ///     Usar convención de nombres snake_case
    /// </summary>
    /// <remarks>
    ///     Este método establece la convención de nombres para tablas, columnas, claves,
    ///     claves foráneas e índices a snake_case.
    /// </remarks>
    public static void UsarConvencionNombresSerpiente(this ModelBuilder constructor)
    {
        foreach (var entidad in constructor.Model.GetEntityTypes())
        {
            // 🏷️ TRADUCE el nombre de la tabla a snake_case y plural
            var nombreTabla = entidad.GetTableName();
            if(!string.IsNullOrEmpty(nombreTabla))
                entidad.SetTableName(nombreTabla.Pluralize().Underscore());
            
            // 🔑 TRADUCE el nombre de las claves primarias
            foreach (var clave in entidad.GetKeys() )
            {
                var nombreClave = clave.GetName();
                if (!string.IsNullOrEmpty(nombreClave))
                    clave.SetName(nombreClave.Underscore());
            }
            // 🔗 TRADUCE el nombre de las claves foráneas
            foreach (var claveExterna in entidad.GetForeignKeys())
            {
                var nombreClaveExterna = claveExterna.GetConstraintName();
                if (!string.IsNullOrEmpty(nombreClaveExterna)) 
                    claveExterna.SetConstraintName(nombreClaveExterna.Underscore());
            }
            
            // 📊 TRADUCE el nombre de los índices
            foreach (var indice in entidad.GetIndexes())
            {
                var nombreIndice = indice.GetDatabaseName();
                if (!string.IsNullOrEmpty(nombreIndice)) 
                    indice.SetDatabaseName(nombreIndice.Underscore());
            }
        }
    }
}