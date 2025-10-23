using WebApplication1.Shared.Domain.Repositories;
using WebApplication1.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace WebApplication1.Shared.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Implementación de la Unidad de Trabajo
/// </summary>
/// <remarks>
///     Esta clase implementa las operaciones básicas para una unidad de trabajo.
///     Requiere que el contexto se pase en el constructor.
/// </remarks>
/// <see cref="IUnitOfWork" />
public class UnidadDeTrabajo(AppDbContext contexto) : IUnidadDeTrabajo
{
    /// <inheritdoc />
    public async Task CompletarAsync()
    {
        await contexto.SaveChangesAsync();
    }
}