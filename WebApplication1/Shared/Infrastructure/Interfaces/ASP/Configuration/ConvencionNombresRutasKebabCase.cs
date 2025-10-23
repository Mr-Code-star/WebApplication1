using Microsoft.AspNetCore.Mvc.ApplicationModels;
using WebApplication1.Shared.Infrastructure.Interfaces.ASP.Configuration.Extensions; 

namespace WebApplication1.Shared.Infrastructure.Interfaces.ASP.Configuration;

/// <summary>
/// Convención para nombrar rutas en formato kebab-case
/// Transforma automáticamente los nombres de controladores a formato kebab-case
/// Ejemplo: "FavoriteSources" → "favorite-sources"
/// </summary>
public class ConvencionNombresRutasKebabCase : IControllerModelConvention
{
    /// <summary>
    /// Reemplaza la plantilla [controller] por el nombre en kebab-case
    /// </summary>
    /// <param name="selector">El selector de ruta actual</param>
    /// <param name="nombre">Nombre del controlador a convertir</param>
    /// <returns>Nuevo modelo de ruta con el nombre convertido</returns>
    private static AttributeRouteModel? ReemplazarPlantillaControlador(SelectorModel selector, string nombre)
    {
        // Si el selector tiene una plantilla de ruta definida
        return selector.AttributeRouteModel != null ? new AttributeRouteModel
        {
            // Reemplaza "[controller]" por el nombre en kebab-case
            // Ejemplo: "api/[controller]" → "api/favorite-sources"
            Template = selector.AttributeRouteModel.Template?.Replace("[controller]", nombre.ConvertirAKebabCase())
        } : null;
    }
    
    /// <summary>
    /// Método principal que aplica la convención a un controlador
    /// Se ejecuta automáticamente para CADA controlador en la aplicación
    /// </summary>
    /// <param name="controlador">El controlador que se está procesando</param>
    public void Apply(ControllerModel controller)
    {
        // 📍 PRIMERO: Procesa los selectores del CONTROLADOR mismo
        // Estos definen las rutas base del controlador
        foreach (var selector in controller.Selectors)
        {
            // Reemplaza "[controller]" en la plantilla de ruta del controlador

            selector.AttributeRouteModel = ReemplazarPlantillaControlador(selector, controller.ControllerName);
        }
        
        // 📍 SEGUNDO: Procesa los selectores de todas las ACCIONES del controlador
        // Estos definen las rutas específicas de cada método (GET, POST, etc.)
        foreach (var selector in controller.Actions.SelectMany(a => a.Selectors))
        {
            // Reemplaza "[controller]" en la plantilla de ruta de cada acción
            selector.AttributeRouteModel = ReemplazarPlantillaControlador(selector, controller.ControllerName);
        }
    }
}