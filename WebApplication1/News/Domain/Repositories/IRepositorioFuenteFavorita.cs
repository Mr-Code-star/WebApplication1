using WebApplication1.News.Domain.Model.Agregates;
using WebApplication1.Shared.Domain.Repositories;

namespace WebApplication1.News.Domain.Repositories;

/// <summary>
/// El contrato del repositorio de fuentes favoritas.
/// </summary>
/// <remarks>
/// Esta interfaz define las operaciones ESPECÍFICAS para el repositorio de fuentes favoritas.
/// Hereda todas las operaciones básicas CRUD de IRepositorioBase y añade métodos especializados.
/// </remarks>
public interface IRepositorioFuenteFavorita : IRepositorioBase<FuenteFavorita>
{
    /// <summary>
    /// Busca fuentes favoritas por Clave API de Noticias
    /// </summary>
    /// <param name="claveApiNoticias">La Clave API de Noticias para buscar</param>
    /// <returns>
    /// Una colección que contiene los objetos de fuentes favoritas si se encuentran, o vacío en caso contrario.
    /// </returns>
    Task<IEnumerable<FuenteFavorita>> BuscarPorClaveApiNoticiasAsync(string claveApiNoticias);
    
    /// <summary>
    /// Busca una fuente favorita por Clave API de Noticias e ID de Fuente
    /// </summary>
    /// <param name="claveApiNoticias">La Clave API de Noticias</param>
    /// <param name="idFuente">El ID de la Fuente</param>
    /// <returns>
    /// El objeto de fuente favorita si se encuentra, o null en caso contrario.
    /// </returns>
    Task<FuenteFavorita?> BuscarPorClaveApiNoticiasYIdFuenteAsync(string claveApiNoticias, string idFuente);
}