using Microsoft.EntityFrameworkCore;
using WebApplication1.News.Application.Internal.CommandServices;
using WebApplication1.News.Application.Internal.QueryServices;
using WebApplication1.News.Domain.Repositories;
using WebApplication1.News.Domain.Services;
using WebApplication1.News.Infrastructure.Repositories;
using WebApplication1.Shared.Domain.Repositories;
using WebApplication1.Shared.Infrastructure.Interfaces.ASP.Configuration;
using WebApplication1.Shared.Infrastructure.Persistence.EFC.Configuration;
using WebApplication1.Shared.Infrastructure.Persistence.EFC.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 📦 SECCIÓN 1: CONFIGURACIÓN DE SERVICIOS
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers(options => options.Conventions.Add(new ConvencionNombresRutasKebabCase()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.EnableAnnotations());

// 🗄️ SECCIÓN 2: CONFIGURACIÓN DE BASE DE DATOS
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<AppDbContext>(
        options =>
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            if (connectionString is null)
                throw new Exception("Database connection string is not set.");
            
            options.UseMySQL(connectionString)
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        });
}
else if (builder.Environment.IsProduction())
{
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
            
        var connectionStringTemplate = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionStringTemplate)) 
            throw new Exception("Database connection string template is not set.");
        
        var connectionString = Environment.ExpandEnvironmentVariables(connectionStringTemplate);
        if (string.IsNullOrEmpty(connectionString))
            throw new Exception("Database connection string is not set.");
        
        options.UseMySQL(connectionString)
            .LogTo(Console.WriteLine, LogLevel.Error)
            .EnableDetailedErrors();
    });
}

// 💉 SECCIÓN 3: INYECCIÓN DE DEPENDENCIAS
builder.Services.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();
builder.Services.AddScoped<IRepositorioFuenteFavorita, RepositorioFuenteFavorita>();
builder.Services.AddScoped<IServicioComandoFuenteFavorita, ServicioComandoFuenteFavorita>();
builder.Services.AddScoped<IServicioConsultaFuenteFavorita, ServicioConsultaFuenteFavorita>();

// 🏗️ SECCIÓN 4: CONSTRUCCIÓN DE LA APLICACIÓN
var app = builder.Build();

// 🗃️ SECCIÓN 5: VERIFICACIÓN Y PREPARACIÓN DE BASE DE DATOS (¡AGREGA ESTO!)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        
        Console.WriteLine("🔍 Verificando base de datos...");
        
        // ✅ ESTO SÍ CREA LA BASE DE DATOS Y TABLAS AUTOMÁTICAMENTE
        var created = context.Database.EnsureCreated();
        Console.WriteLine(created ? "✅ Base de datos y tablas creadas" : "✅ Base de datos ya existía");
        
        // Verificar que la tabla existe
        var tableExists = await context.Database.CanConnectAsync();
        Console.WriteLine(tableExists ? "✅ Conexión a BD exitosa" : "❌ No se pudo conectar a BD");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error crítico al configurar la base de datos: {ex.Message}");
        if (app.Environment.IsDevelopment()) throw;
    }
}

// 🌐 SECCIÓN 6: CONFIGURACIÓN DEL PIPELINE HTTP
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "News API v1");
    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// 🚀 INICIA LA APLICACIÓN
Console.WriteLine("🚀 Aplicación iniciando...");
app.Run();