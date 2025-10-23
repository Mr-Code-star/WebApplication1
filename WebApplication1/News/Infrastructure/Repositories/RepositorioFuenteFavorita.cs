using Microsoft.EntityFrameworkCore;
using WebApplication1.News.Domain.Model.Agregates;
using WebApplication1.News.Domain.Repositories;
using WebApplication1.Shared.Infrastructure.Persistence.EFC.Configuration;
using WebApplication1.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace WebApplication1.News.Infrastructure.Repositories;

/// <summary>
/// Repositorio de fuentes favoritas
/// </summary>
/// <remarks>
/// Esta clase implementa las operaciones ESPECÍFICAS para un repositorio de fuentes favoritas.
/// Hereda todas las operaciones básicas (CRUD) del RepositorioBase y añade métodos especializados.
/// </remarks>
/// <param name="contexto">La instancia de AppDbContext.</param>
public class RepositorioFuenteFavorita(AppDbContext contexto)
    : RepositorioBase<FuenteFavorita>(contexto), IRepositorioFuenteFavorita
{
    /// <summary>
    /// Busca todas las fuentes favoritas por clave API de noticias
    /// </summary>
    /// <param name="claveApiNoticias">La clave API para filtrar</param>
    /// <returns>Una lista de todas las fuentes favoritas del usuario</returns>
    /// <inheritdoc />
    public async Task<IEnumerable<FuenteFavorita>> BuscarPorClaveApiNoticiasAsync(string claveApiNoticias)
    {
        // 🔍 CONSULTA: "Dame TODAS las fuentes favoritas de este usuario"
        return await Contexto.Set<FuenteFavorita>()
            .Where(f => f.ClaveApiNoticias == claveApiNoticias)
            .ToListAsync();
    }

    /// <summary>
    /// Busca una fuente favorita específica por clave API y ID de fuente
    /// </summary>
    /// <param name="claveApiNoticias">La clave API del usuario</param>
    /// <param name="idFuente">El ID de la fuente a buscar</param>
    /// <returns>La fuente favorita si existe, o null si no se encuentra</returns>
    /// <inheritdoc />
    public async Task<FuenteFavorita?> BuscarPorClaveApiNoticiasYIdFuenteAsync(string claveApiNoticias, string idFuente)
    {
        // 🔍 CONSULTA: "Dame EXACTAMENTE esta fuente favorita del usuario"
        return await Contexto.Set<FuenteFavorita>()
            .FirstOrDefaultAsync(f => f.ClaveApiNoticias == claveApiNoticias && f.IdFuente == idFuente);
    }
}