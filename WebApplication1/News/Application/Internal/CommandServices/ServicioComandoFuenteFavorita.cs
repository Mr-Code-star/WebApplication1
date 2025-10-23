using WebApplication1.News.Domain.Model.Agregates;
using WebApplication1.News.Domain.Model.commands;
using WebApplication1.News.Domain.Repositories;
using WebApplication1.News.Domain.Services;
using WebApplication1.Shared.Domain.Repositories;

namespace WebApplication1.News.Application.Internal.CommandServices;

/// <summary>
/// Servicio de comandos para fuentes favoritas.
/// </summary>
/// <remarks>
/// Esta clase implementa las operaciones básicas para un servicio de comandos de fuentes favoritas.
/// Se encarga de manejar la lógica de negocio para CREAR fuentes favoritas.
/// </remarks>
/// <param name="repositorioFuenteFavorita">La instancia de RepositorioFuenteFavorita</param>
/// <param name="unidadDeTrabajo">La instancia de UnidadDeTrabajo</param>
public class ServicioComandoFuenteFavorita(IRepositorioFuenteFavorita repositorioFuenteFavorita, IUnidadDeTrabajo unidadDeTrabajo)
    : IServicioComandoFuenteFavorita
{
    /// <inheritdoc />
    public async Task<FuenteFavorita?> Manejar(ComandoCrearFuenteFavorita comando)
    {
        // 🔍 PASO 1: Verificar si YA EXISTE una fuente favorita con la misma clave API y fuente
        var fuenteFavoritaExistente =
            await repositorioFuenteFavorita.BuscarPorClaveApiNoticiasYIdFuenteAsync(comando.ClaveApiNoticias, comando.IdFuente);
        
        // ❌ SI ya existe, lanzar error (evitar duplicados)
        if (fuenteFavoritaExistente != null)
            throw new Exception("Ya existe una fuente favorita con este IdFuente para la ClaveApiNoticias dada");

        // 🆕 PASO 2: Crear NUEVA fuente favorita a partir del comando
        var nuevaFuenteFavorita = new FuenteFavorita(comando);
        
        try
        {
            // 💾 PASO 3: Guardar la nueva fuente favorita en la base de datos
            await repositorioFuenteFavorita.AgregarAsync(nuevaFuenteFavorita);
            
            // ✅ PASO 4: Confirmar todos los cambios en la base de datos
            await unidadDeTrabajo.CompletarAsync();
        }
        catch (Exception e)
        {
            // 🚨 PASO 5: Si hay algún error, devolver null (fallo silencioso)
            return null;
        }

        // 🎉 PASO 6: Si todo sale bien, devolver la fuente favorita creada
        return nuevaFuenteFavorita;
    }
}
