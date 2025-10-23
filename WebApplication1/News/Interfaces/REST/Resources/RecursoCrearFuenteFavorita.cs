namespace WebApplication1.News.Interfaces.REST.Resources;


/// <summary>
/// Representa los datos requeridos para crear una fuente favorita.
/// </summary>
/// <param name="ClaveApiNoticias">La clave de API de noticias</param>
/// <param name="IdFuente">El ID de la fuente</param>
public record RecursoCrearFuenteFavorita(
    string ClaveApiNoticias,
    string IdFuente
);