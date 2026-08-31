
using System;

namespace WebApplication1.shared.infrastructure.config;


/// <summary>
/// Configuración para el umbral de omisión de dosis
/// </summary>
public static class DoseConfig
{
    /// <summary>
    /// Propiedad estática (para compatibilidad con código existente)
    /// Este valor se calcula UNA SOLA VEZ al iniciar la aplicación
    /// </summary>
    public static double OmissionThresholdHours { get; } = CalculateThreshold();

    /// <summary>
    /// Método dinámico para obtener el umbral (RECOMENDADO)
    /// Este método se ejecuta CADA VEZ que se necesita el valor
    /// Permite cambios en tiempo real si se modifica la variable de entorno
    /// </summary>
    public static double GetOmissionThresholdHours()
    {
        // Obtener el entorno actual
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") 
                  ?? "Production";

        // ==============================================
        // LÓGICA PARA DESARROLLO
        // ==============================================
        if (env.Equals("Development", StringComparison.OrdinalIgnoreCase) || 
            env.Equals("Test", StringComparison.OrdinalIgnoreCase))
        {
            // Leer variable de entorno (en MINUTOS)
            var devThreshold = Environment.GetEnvironmentVariable("DEV_OMISSION_MINUTES");

            // Si el usuario configuró un valor personalizado
            if (!string.IsNullOrEmpty(devThreshold) && double.TryParse(devThreshold, out var threshold))
            {
                // Convertir minutos → horas (porque el sistema trabaja en horas)
                return threshold / 60;
            }

            // Si NO hay variable, usar 1 minuto por defecto
            return 1.0 / 60; // 1 minuto en desarrollo
        }

        // ==============================================
        // LÓGICA PARA PRODUCCIÓN
        // ==============================================
        return 24; // 24 horas en producción
    }

    private static double CalculateThreshold()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") 
                  ?? "Production";

        if (env.Equals("Development", StringComparison.OrdinalIgnoreCase) || 
            env.Equals("Test", StringComparison.OrdinalIgnoreCase))
        {
            var devThreshold = Environment.GetEnvironmentVariable("DEV_OMISSION_MINUTES");

            if (!string.IsNullOrEmpty(devThreshold) && double.TryParse(devThreshold, out var threshold))
            {
                return threshold / 60;
            }

            return 1.0 / 60;
        }

        return 24;
    }
}