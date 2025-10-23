using CatchUpPlatform.API.News.Domain.Model.Queries;
using WebApplication1.News.Domain.Model.Agregates;
using WebApplication1.News.Domain.Model.queries;
using WebApplication1.News.Domain.Repositories;
using WebApplication1.News.Domain.Services;

namespace WebApplication1.News.Application.Internal.QueryServices;

/// <summary>
/// Servicio de consultas para fuentes favoritas.
/// </summary>
/// <remarks>
/// Esta clase implementa las operaciones básicas para un servicio de consultas de fuentes favoritas.
/// Se encarga de manejar las diferentes formas de BUSCAR y CONSULTAR fuentes favoritas.
/// </remarks>
/// <param name="repositorioFuenteFavorita">La instancia de RepositorioFuenteFavorita</param>
public class ServicioConsultaFuenteFavorita(IRepositorioFuenteFavorita repositorioFuenteFavorita)
    : IServicioConsultaFuenteFavorita
{
    /// <inheritdoc />
    public async Task<IEnumerable<FuenteFavorita>> Manejar(ConsultaObtenerFuentesFavoritasPorClaveApi consulta)
    {
        // 🔍 BUSCAR: "Dame TODAS las fuentes favoritas de este usuario"
        // Ejemplo: Todas las fuentes del usuario "clave-123"
        return await repositorioFuenteFavorita.BuscarPorClaveApiNoticiasAsync(consulta.ClaveApiNoticias);
    }

    /// <inheritdoc />
    public async Task<FuenteFavorita?> Manejar(ConsultaObtenerFuenteFavoritaPorClaveApiYIdFuente consulta)
    {
        // 🔍 BUSCAR: "Dame EXACTAMENTE esta fuente favorita del usuario"
        // Ejemplo: La fuente "bbc-news" del usuario "clave-123"
        return await repositorioFuenteFavorita.BuscarPorClaveApiNoticiasYIdFuenteAsync(consulta.ClaveApiNoticias, consulta.IdFuente);
    }

    /// <inheritdoc />
    public async Task<FuenteFavorita?> Manejar(ConsultaObtenerFuenteFavoritaPorId consulta)
    {
        // 🔍 BUSCAR: "Dame la fuente favorita con este ID único"
        // Ejemplo: La fuente con ID número 15
        return await repositorioFuenteFavorita.BuscarPorIdAsync(consulta.Id);
    }
}