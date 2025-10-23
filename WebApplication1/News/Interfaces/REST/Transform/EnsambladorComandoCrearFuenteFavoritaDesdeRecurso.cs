using WebApplication1.News.Domain.Model.commands;
using WebApplication1.News.Interfaces.REST.Resources;

namespace WebApplication1.News.Interfaces.REST.Transform;

public static class EnsambladorComandoCrearFuenteFavoritaDesdeRecurso
{
    /// <summary>
    /// Convierte un RecursoCrearFuenteFavorita en un ComandoCrearFuenteFavorita.
    /// </summary>
    /// <param name="recurso">El recurso RecursoCrearFuenteFavorita</param>
    /// <returns>
    /// Un ComandoCrearFuenteFavorita convertido desde el RecursoCrearFuenteFavorita
    /// </returns>
    public static ComandoCrearFuenteFavorita AComandoDesdeRecurso(RecursoCrearFuenteFavorita recurso) =>
        new ComandoCrearFuenteFavorita(recurso.ClaveApiNoticias, recurso.IdFuente);  
}