namespace WebApplication1.News.Interfaces.REST.Resources;

/// <summary>
/// Representa los datos proporcionados por el servidor sobre una fuente favorita.
/// </summary>
/// <param name="Id">El ID generado por el servidor</param>
/// <param name="ClaveApiNoticias">La clave de API de noticias</param>
/// <param name="IdFuente">El ID de la fuente</param>
/// 
public record RecursoFuenteFavorita(int Id, string ClaveApiNoticias, string IdFuente);