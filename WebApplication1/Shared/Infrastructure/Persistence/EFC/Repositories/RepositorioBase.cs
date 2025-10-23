using Microsoft.EntityFrameworkCore;
using WebApplication1.Shared.Domain.Repositories;
using WebApplication1.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace WebApplication1.Shared.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Repositorio base para todos los repositorios
/// </summary>
/// <remarks>
///     Esta clase implementa las operaciones CRUD básicas para todos los repositorios.
///     Requiere que el tipo de entidad se pase como parámetro genérico.
///     También requiere que el contexto se pase en el constructor.
/// </remarks>
public class RepositorioBase<TEntidad> : IRepositorioBase<TEntidad> where TEntidad : class
{
    protected readonly AppDbContext Contexto;
    
    protected RepositorioBase(AppDbContext contexto)
    {
        Contexto = contexto;
    }
    
    /// <inheritdoc />
    public async Task AgregarAsync(TEntidad entidad)
    {
        await Contexto.Set<TEntidad>().AddAsync(entidad);
    }

    /// <inheritdoc />
    public async Task<TEntidad?> BuscarPorIdAsync(int id)
    {
        return await Contexto.Set<TEntidad>().FindAsync(id);
    }

    /// <inheritdoc />
    public void Actualizar(TEntidad entidad)
    {
        Contexto.Set<TEntidad>().Update(entidad);
    }

    /// <inheritdoc />
    public void Eliminar(TEntidad entidad)
    {
        Contexto.Set<TEntidad>().Remove(entidad);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TEntidad>> ListarAsync()
    {
        return await Contexto.Set<TEntidad>().ToListAsync();
    }
}