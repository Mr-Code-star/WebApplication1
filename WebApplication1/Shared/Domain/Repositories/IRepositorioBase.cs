namespace WebApplication1.Shared.Domain.Repositories;

/// <summary>
///     Interfaz base de repositorio para todos los repositorios
/// </summary>
/// <remarks>
///     Esta interfaz define las operaciones básicas CRUD para todos los repositorios
/// </remarks>
/// <typeparam name="TEntidad">El Tipo de Entidad</typeparam>
/// 
public interface IRepositorioBase<TEntidad>
{
    /// <summary>
    ///     Agregar entidad al repositorio
    /// </summary>
    /// <param name="entidad">Objeto entidad a agregar</param>
    /// <returns></returns>
    Task AgregarAsync(TEntidad entidad);
    
    /// <summary>
    ///     Buscar entidad por ID
    /// </summary>
    /// <param name="id">El ID de la entidad a buscar</param>
    /// <returns>Objeto entidad si se encuentra</returns>
    Task<TEntidad?> BuscarPorIdAsync(int id);
    
    /// <summary>
    ///     Actualizar entidad
    /// </summary>
    /// <param name="entidad">El objeto entidad a actualizar</param>
    void Actualizar(TEntidad entidad);
    
    /// <summary>
    ///     Eliminar una entidad
    /// </summary>
    /// <param name="entidad">El objeto entidad a eliminar</param>
    void Eliminar(TEntidad entidad);
    
    /// <summary>
    ///     Obtener todas las entidades
    /// </summary>
    /// <returns>Una colección que contiene todos los objetos entidad</returns>
    Task<IEnumerable<TEntidad>> ListarAsync();
    
}