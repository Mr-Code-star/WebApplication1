namespace WebApplication1.Shared.Domain.Repositories;

/// <summary>
///     Interfaz de Unidad de Trabajo
/// </summary>
/// <remarks>
///     Esta interfaz define las operaciones básicas para una unidad de trabajo
/// </remarks>
public interface IUnidadDeTrabajo
{
    /// <summary>
    ///     Confirmar cambios en la base de datos
    /// </summary>
    Task CompletarAsync();
}