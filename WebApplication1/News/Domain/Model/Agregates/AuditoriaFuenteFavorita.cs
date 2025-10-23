// 📦 PAQUETE: Viene incluido con .NET - No necesita descarga
// FUNCIÓN: Proporciona "etiquetas" para decirle a la base de datos cómo guardar los datos
using System.ComponentModel.DataAnnotations.Schema;

// 📦 PAQUETE: Externo - Lo tuviste que descargar via NuGet
// FUNCIÓN: Proporciona un "contrato estándar" para fechas de creación y actualización
using EntityFrameworkCore.CreatedUpdatedDate.Contracts;

namespace WebApplication1.News.Domain.Model.Agregates;

/// <summary>
/// FuenteFavorita con Fechas de Creación y Actualización
/// </summary>
/// <remarks>
/// Esta clase agrega "fechas y horas" a las fuentes favoritas para saber cuándo se crearon y actualizaron
/// </remarks>
public partial class FuenteFavorita : IEntityWithCreatedUpdatedDate // ✅ FIRMA EL CONTRATO
{
    // 🏷️ ETIQUETA: Le dice a la base de datos "guarda esto en la columna 'CreatedAt'"
    // 📊 EN BASE DE DATOS: Se guardará como "CreatedAt"
    // 💾 PROPÓSITO: Registrar cuándo se creó esta fuente favorita por primera vez
    [Column("CreatedAt")]  
    public DateTimeOffset? CreatedDate  { get; set; } // 📅 Cuándo se creó

    // 🏷️ ETIQUETA: Le dice a la base de datos "guarda esto en la columna 'UpdatedAt'"  
    // 📊 EN BASE DE DATOS: Se guardará como "UpdatedAt"
    // 💾 PROPÓSITO: Registrar cuándo se modificó esta fuente favorita por última vez
    [Column("UpdatedAt")] 
    public DateTimeOffset? UpdatedDate { get; set; } // 🔄 Cuándo se actualizó
}