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

        // 1) Aplica tu convención global (por si otras entidades la usan)
        builder.UsarConvencionNombresSerpiente();

        // 2) Para FuenteFavorita, FUERZA tabla y columnas exactas que tiene tu BD
        var e = builder.Entity<FuenteFavorita>();

        // Si tu tabla en MySQL se llama "FuenteFavoritas" (con mayúsculas), usa exactamente ese nombre:
        e.ToTable("fuente_favoritas");               // <- o "fuente_favoritas" si ese es el nombre real en tu server

        e.HasKey(f => f.Id);
        e.Property(f => f.Id)
            .HasColumnName("Id")
            .IsRequired()
            .ValueGeneratedOnAdd();

        e.Property(f => f.IdFuente)
            .HasColumnName("IdFuente")
            .IsRequired();

        e.Property(f => f.ClaveApiNoticias)
            .HasColumnName("ClaveApiNoticias")
            .IsRequired();
    }
}