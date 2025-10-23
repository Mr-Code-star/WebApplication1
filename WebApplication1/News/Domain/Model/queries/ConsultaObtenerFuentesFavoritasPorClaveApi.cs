// Define el espacio de nombres para las consultas del dominio de noticias
namespace WebApplication1.News.Domain.Model.queries;

/// <summary>
///     Consulta para obtener todas las fuentes favoritas por NewsApiKey
///     (Como cuando pides ver tu lista de favoritos)
/// </summary>
/// <param name="ClaveApiNoticias">La clave de API para buscar (tu "llave de acceso")</param>
/// 
public record ConsultaObtenerFuentesFavoritasPorClaveApi(string ClaveApiNoticias);
