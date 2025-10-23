// Define el espacio de nombres para las consultas del dominio de noticias
namespace WebApplication1.News.Domain.Model.queries;

/// <summary>
///     Consulta para obtener una fuente favorita por ClaveApiNoticias y IdFuente
///     (Como cuando buscas una combinación exacta)
/// </summary>
/// <param name="ClaveApiNoticias">La clave de API para buscar</param>
/// <param name="IdFuente">El ID de la fuente a buscar</param>
public record ConsultaObtenerFuenteFavoritaPorClaveApiYIdFuente(string ClaveApiNoticias, string IdFuente);
