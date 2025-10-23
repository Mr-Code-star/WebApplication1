using CatchUpPlatform.API.News.Domain.Model.Queries;
using WebApplication1.News.Domain.Model.Agregates;
using WebApplication1.News.Domain.Model.queries;

namespace WebApplication1.News.Domain.Services;

/// <summary>
///     Interfaz para el Servicio de Consultas de Fuentes Favoritas
/// </summary>
/// <remarks>
///     Esta interfaz define las operaciones básicas para el servicio de consultas de fuentes favoritas
/// </remarks>
public interface IServicioConsultaFuenteFavorita
{
    /// <summary>
    ///     Maneja la consulta ObtenerTodasLasFuentesFavoritasPorClaveApi
    /// </summary>
    /// <remarks>
    ///     Este método maneja la consulta para obtener todas las fuentes favoritas.
    ///     Retorna todas las fuentes favoritas para la ClaveApiNoticias dada.
    /// </remarks>
    /// <param name="consulta">La consulta ObtenerTodasLasFuentesFavoritasPorClaveApi</param>
    /// <returns>Una colección que contiene los objetos FuenteFavorita</returns>
    Task<IEnumerable<FuenteFavorita>> Manejar(ConsultaObtenerFuentesFavoritasPorClaveApi consulta);

    /// <summary>
    ///     Maneja la consulta ObtenerFuenteFavoritaPorClaveApiYIdFuente
    /// </summary>
    /// <remarks>
    ///     Este método maneja la consulta para obtener una fuente favorita específica.
    ///     Retorna la fuente favorita para la ClaveApiNoticias e IdFuente dados.
    /// </remarks>
    /// <param name="consulta">La consulta ObtenerFuenteFavoritaPorClaveApiYIdFuente</param>
    /// <returns>El objeto FuenteFavorita si se encuentra, o null en caso contrario</returns>
    Task<FuenteFavorita?> Manejar(ConsultaObtenerFuenteFavoritaPorClaveApiYIdFuente consulta);

    /// <summary>
    ///     Maneja la consulta ObtenerFuenteFavoritaPorId
    /// </summary>
    /// <remarks>
    ///     Este método maneja la consulta para obtener una fuente favorita por su ID.
    ///     Retorna la fuente favorita para el ID dado.
    /// </remarks>
    /// <param name="consulta">La consulta ObtenerFuenteFavoritaPorId</param>
    /// <returns>
    ///     El objeto FuenteFavorita si se encuentra, o null en caso contrario
    /// </returns>
    Task<FuenteFavorita?> Manejar(ConsultaObtenerFuenteFavoritaPorId consulta);
}