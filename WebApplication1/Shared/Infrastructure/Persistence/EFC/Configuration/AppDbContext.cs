using EntityFrameworkCore.CreatedUpdatedDate.Extensions;
using Microsoft.EntityFrameworkCore;
using WebApplication1.News.Domain.Model.Agregates;
using WebApplication1.Shared.Infrastructure.Persistence.EFC.Configuration.Extentions;

namespace WebApplication1.Shared.Infrastructure.Persistence.EFC.Configuration;

/// <summary>
///     Contexto de base de datos de la aplicación
/// </summary>
public class AppDbContext(DbContextOptions opciones) : DbContext(opciones)
{
    // 🗃️ AGREGA ESTA LÍNEA: DbSet para la entidad FuenteFavorita
    public DbSet<FuenteFavorita> FuenteFavoritas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        // Agrega el interceptor de fechas de creación y actualización
        builder.AddCreatedUpdatedInterceptor();
        base.OnConfiguring(builder);
    }
        
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // 🗂️ CONFIGURACIÓN de la entidad FuenteFavorita
        builder.Entity<FuenteFavorita>().HasKey(f => f.Id); // 🔑 Establece ID como clave primaria
        builder.Entity<FuenteFavorita>().Property(f => f.Id).IsRequired().ValueGeneratedOnAdd(); // ⚡ ID requerido y generado automáticamente
        builder.Entity<FuenteFavorita>().Property(f => f.IdFuente).IsRequired(); // 📰 IdFuente requerido
        builder.Entity<FuenteFavorita>().Property(f => f.ClaveApiNoticias).IsRequired(); // 🔑 ClaveApiNoticias requerido
        
        // 🐍 Aplica la convención de nombres snake_case
        builder.UsarConvencionNombresSerpiente();
    }
}