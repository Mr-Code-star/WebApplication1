using WebApplication1.News.Domain.Model.Agregates;
using WebApplication1.News.Domain.Model.commands;

namespace WebApplication1.News.Domain.Services;

/// <summary>
/// Servicio de Comandos para Fuentes Favoritas
/// </summary>
/// <remarks>
/// Este es el "contrato" que define CÓMO se deben construir las fuentes favoritas
/// </remarks>
public interface IServicioComandoFuenteFavorita
{
    /// <summary>
    /// Maneja el comando de crear fuente favorita
    /// </summary>
    /// <remarks>
    /// Este método es el "encargado de construcción" que:
    /// 1. ✅ Verifica si la fuente favorita YA EXISTE
    /// 2. ✅ Si NO existe, construye una nueva fuente favorita
    /// 3. ✅ La guarda en la base de datos
    /// </remarks>
    /// <param name="comando">El "papelito de pedido" para crear fuente favorita</param>
    /// <returns>La fuente favorita construida o null si hubo error</returns>
    Task<FuenteFavorita?> Manejar(ComandoCrearFuenteFavorita comando);
}