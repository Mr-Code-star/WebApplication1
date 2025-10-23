using WebApplication1.News.Domain.Model.Agregates;
using WebApplication1.News.Interfaces.REST.Resources;

namespace WebApplication1.News.Interfaces.REST.Transform;

/// <summary>
/// Convierte una FuenteFavorita en un RecursoFuenteFavorita.
/// </summary>
public static class EnsambladorRecursoFuenteFavoritaDesdeEntidad
{
    /// <summary>
    /// Convierte una FuenteFavorita en un RecursoFuenteFavorita.
    /// </summary>
    /// <param name="entidad">La entidad FuenteFavorita</param>
    /// <returns>
    /// Un RecursoFuenteFavorita convertido desde la FuenteFavorita
    /// </returns>
    public static RecursoFuenteFavorita ARecursoDesdeEntidad(FuenteFavorita entidad) =>
        new RecursoFuenteFavorita(entidad.Id, entidad.ClaveApiNoticias, entidad.IdFuente);
}