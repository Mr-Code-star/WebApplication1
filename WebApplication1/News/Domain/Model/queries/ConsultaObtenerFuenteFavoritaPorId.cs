// Define el espacio de nombres para las consultas del dominio de noticias
namespace CatchUpPlatform.API.News.Domain.Model.Queries;

/// <summary>
///     Consulta para obtener una fuente favorita por su ID
///     (Como cuando buscas una fuente específica en tus favoritos)
/// </summary>
/// <param name="Id">El ID de la fuente a buscar (su "número único de identificación")</param>
public record ConsultaObtenerFuenteFavoritaPorId(int Id);