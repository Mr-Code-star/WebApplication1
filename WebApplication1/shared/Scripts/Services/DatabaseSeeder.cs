using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using WebApplication1.shared.Scripts.Models;

namespace WebApplication1.Services;

/// <summary>
/// Servicio de seed para la base de datos
/// Se ejecuta automáticamente al iniciar la aplicación
/// </summary>
public class DatabaseSeeder : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        IServiceScopeFactory scopeFactory,
        ILogger<DatabaseSeeder> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // ✅ Crear un scope para obtener servicios scoped
        using var scope = _scopeFactory.CreateScope();
        
        // ✅ Obtener IMongoDatabase del scope
        var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
        var foodItemCollection = database.GetCollection<FoodItem>("fooditems");

        try
        {
            _logger.LogInformation("🔄 Iniciando seed de alimentos...");

            // Verificar si ya existen datos
            var existingCount = await foodItemCollection.CountDocumentsAsync(
                FilterDefinition<FoodItem>.Empty,
                cancellationToken: cancellationToken
            );

            if (existingCount > 0)
            {
                _logger.LogInformation("✅ Alimentos ya existentes en la base de datos. Encontrados: {Count}", existingCount);
                return;
            }

            // Obtener los alimentos
            var foodItems = GetFoodItems();

            // Insertar en lotes (mejor performance)
            var batchSize = 50;
            for (int i = 0; i < foodItems.Count; i += batchSize)
            {
                var batch = foodItems.Skip(i).Take(batchSize);
                await foodItemCollection.InsertManyAsync(batch, cancellationToken: cancellationToken);
                _logger.LogInformation("📦 Lote {BatchNumber} insertado: {Count} items", (i / batchSize) + 1, batch.Count());
            }

            _logger.LogInformation("✅ Seed completado exitosamente. Insertados: {Count} items", foodItems.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error durante el seed de la base de datos");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("🛑 DatabaseSeeder detenido");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Lista completa de alimentos
    /// </summary>
    private List<FoodItem> GetFoodItems()
    {
        return new List<FoodItem>
        {
            // ==========================================
            // CARNES (MEAT)
            // ==========================================
            new FoodItem 
            { 
                FoodId = "FOOD_001", 
                Name = "Sangrecita de pollo", 
                NutrientContent = new NutrientContent { IronMg = 29.5, IronType = "hemo" },
                IsInhibitor = false, 
                Category = "MEAT" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_002", 
                Name = "Bazo de res", 
                NutrientContent = new NutrientContent { IronMg = 14.0, IronType = "hemo" },
                IsInhibitor = false, 
                Category = "MEAT" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_003", 
                Name = "Hígado de pollo", 
                NutrientContent = new NutrientContent { IronMg = 8.5, IronType = "hemo" },
                IsInhibitor = false, 
                Category = "MEAT" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_004", 
                Name = "Hígado de res", 
                NutrientContent = new NutrientContent { IronMg = 6.5, IronType = "hemo" },
                IsInhibitor = false, 
                Category = "MEAT" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_005", 
                Name = "Carne de res", 
                NutrientContent = new NutrientContent { IronMg = 2.7, IronType = "hemo" },
                IsInhibitor = false, 
                Category = "MEAT" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_006", 
                Name = "Pavo", 
                NutrientContent = new NutrientContent { IronMg = 1.8, IronType = "hemo" },
                IsInhibitor = false, 
                Category = "MEAT" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_007", 
                Name = "Huevo entero cocido", 
                NutrientContent = new NutrientContent { IronMg = 1.8, IronType = "no-hemo" },
                IsInhibitor = false, 
                Category = "MEAT" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_008", 
                Name = "Pollo", 
                NutrientContent = new NutrientContent { IronMg = 1.3, IronType = "hemo" },
                IsInhibitor = false, 
                Category = "MEAT" 
            },

            // ==========================================
            // PESCADOS (FISH)
            // ==========================================
            new FoodItem 
            { 
                FoodId = "FOOD_009", 
                Name = "Anchoveta", 
                NutrientContent = new NutrientContent { IronMg = 3.2, IronType = "hemo" },
                IsInhibitor = false, 
                Category = "FISH" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_010", 
                Name = "Sardina en conserva", 
                NutrientContent = new NutrientContent { IronMg = 2.9, IronType = "hemo" },
                IsInhibitor = false, 
                Category = "FISH" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_011", 
                Name = "Atún en conserva", 
                NutrientContent = new NutrientContent { IronMg = 1.9, IronType = "hemo" },
                IsInhibitor = false, 
                Category = "FISH" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_012", 
                Name = "Bonito", 
                NutrientContent = new NutrientContent { IronMg = 1.5, IronType = "hemo" },
                IsInhibitor = false, 
                Category = "FISH" 
            },

            // ==========================================
            // LEGUMBRES (LEGUME)
            // ==========================================
            new FoodItem 
            { 
                FoodId = "FOOD_013", 
                Name = "Lentejas cocidas", 
                NutrientContent = new NutrientContent { IronMg = 3.3, IronType = "no-hemo" },
                IsInhibitor = false, 
                Category = "LEGUME" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_014", 
                Name = "Garbanzos cocidos", 
                NutrientContent = new NutrientContent { IronMg = 2.9, IronType = "no-hemo" },
                IsInhibitor = false, 
                Category = "LEGUME" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_015", 
                Name = "Pallares cocidos", 
                NutrientContent = new NutrientContent { IronMg = 2.5, IronType = "no-hemo" },
                IsInhibitor = false, 
                Category = "LEGUME" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_016", 
                Name = "Frijoles cocidos", 
                NutrientContent = new NutrientContent { IronMg = 2.1, IronType = "no-hemo" },
                IsInhibitor = false, 
                Category = "LEGUME" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_017", 
                Name = "Arvejas cocidas", 
                NutrientContent = new NutrientContent { IronMg = 1.8, IronType = "no-hemo" },
                IsInhibitor = false, 
                Category = "LEGUME" 
            },

            // ==========================================
            // VEGETALES (VEGETABLE)
            // ==========================================
            new FoodItem 
            { 
                FoodId = "FOOD_018", 
                Name = "Espinaca cocida", 
                NutrientContent = new NutrientContent { IronMg = 2.8, IronType = "no-hemo" },
                IsInhibitor = false, 
                Category = "VEGETABLE" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_019", 
                Name = "Acelga cocida", 
                NutrientContent = new NutrientContent { IronMg = 1.8, IronType = "no-hemo" },
                IsInhibitor = false, 
                Category = "VEGETABLE" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_020", 
                Name = "Brócoli cocido", 
                NutrientContent = new NutrientContent { IronMg = 0.7, IronType = "no-hemo" },
                IsInhibitor = false, 
                Category = "VEGETABLE" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_021", 
                Name = "Camote cocido", 
                NutrientContent = new NutrientContent { IronMg = 0.7, IronType = "no-hemo" },
                IsInhibitor = false, 
                Category = "VEGETABLE" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_022", 
                Name = "Papa cocida", 
                NutrientContent = new NutrientContent { IronMg = 0.5, IronType = "no-hemo" },
                IsInhibitor = false, 
                Category = "VEGETABLE" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_023", 
                Name = "Zanahoria cocida", 
                NutrientContent = new NutrientContent { IronMg = 0.4, IronType = "no-hemo" },
                IsInhibitor = false, 
                Category = "VEGETABLE" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_024", 
                Name = "Zapallo cocido", 
                NutrientContent = new NutrientContent { IronMg = 0.4, IronType = "no-hemo" },
                IsInhibitor = false, 
                Category = "VEGETABLE" 
            },

            // ==========================================
            // CEREALES (GRAIN)
            // ==========================================
            new FoodItem 
            { 
                FoodId = "FOOD_025", 
                Name = "Kiwicha cocida", 
                NutrientContent = new NutrientContent { IronMg = 3.1, IronType = "no-hemo" },
                IsInhibitor = false, 
                Category = "GRAIN" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_026", 
                Name = "Pan de trigo", 
                NutrientContent = new NutrientContent { IronMg = 2.5, IronType = "no-hemo" },
                IsInhibitor = false, 
                Category = "GRAIN" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_027", 
                Name = "Avena cocida", 
                NutrientContent = new NutrientContent { IronMg = 1.7, IronType = "no-hemo" },
                IsInhibitor = false, 
                Category = "GRAIN" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_028", 
                Name = "Quinua cocida", 
                NutrientContent = new NutrientContent { IronMg = 1.5, IronType = "no-hemo" },
                IsInhibitor = false, 
                Category = "GRAIN" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_029", 
                Name = "Arroz cocido", 
                NutrientContent = new NutrientContent { IronMg = 0.2, IronType = "no-hemo" },
                IsInhibitor = false, 
                Category = "GRAIN" 
            },

            // ==========================================
            // FRUTAS (FRUIT)
            // ==========================================
            new FoodItem 
            { 
                FoodId = "FOOD_030", 
                Name = "Lúcuma", 
                NutrientContent = new NutrientContent { IronMg = 0.4, IronType = "no-hemo" },
                IsInhibitor = false, 
                Category = "FRUIT" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_031", 
                Name = "Plátano", 
                NutrientContent = new NutrientContent { IronMg = 0.3, IronType = "no-hemo" },
                IsInhibitor = false, 
                Category = "FRUIT" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_032", 
                Name = "Naranja", 
                NutrientContent = new NutrientContent { IronMg = 0.1, IronType = "no-hemo" },
                IsInhibitor = false, 
                Category = "FRUIT" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_033", 
                Name = "Mandarina", 
                NutrientContent = new NutrientContent { IronMg = 0.1, IronType = "no-hemo" },
                IsInhibitor = false, 
                Category = "FRUIT" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_034", 
                Name = "Mango", 
                NutrientContent = new NutrientContent { IronMg = 0.1, IronType = "no-hemo" },
                IsInhibitor = false, 
                Category = "FRUIT" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_035", 
                Name = "Papaya", 
                NutrientContent = new NutrientContent { IronMg = 0.1, IronType = "no-hemo" },
                IsInhibitor = false, 
                Category = "FRUIT" 
            },

            // ==========================================
            // LÁCTEOS (DAIRY) - INHIBIDORES
            // ==========================================
            new FoodItem 
            { 
                FoodId = "FOOD_036", 
                Name = "Queso fresco", 
                NutrientContent = new NutrientContent { IronMg = 0.2, IronType = "no-hemo" },
                IsInhibitor = true, 
                Category = "DAIRY" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_037", 
                Name = "Leche de vaca", 
                NutrientContent = new NutrientContent { IronMg = 0.1, IronType = "no-hemo" },
                IsInhibitor = true, 
                Category = "DAIRY" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_038", 
                Name = "Yogur", 
                NutrientContent = new NutrientContent { IronMg = 0.1, IronType = "no-hemo" },
                IsInhibitor = true, 
                Category = "DAIRY" 
            },

            // ==========================================
            // BEBIDAS (BEVERAGE)
            // ==========================================
            new FoodItem 
            { 
                FoodId = "FOOD_039", 
                Name = "Té", 
                NutrientContent = new NutrientContent { IronMg = 0.0, IronType = "no-hemo" },
                IsInhibitor = true, 
                Category = "BEVERAGE" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_040", 
                Name = "Café", 
                NutrientContent = new NutrientContent { IronMg = 0.0, IronType = "no-hemo" },
                IsInhibitor = true, 
                Category = "BEVERAGE" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_041", 
                Name = "Jugo de naranja", 
                NutrientContent = new NutrientContent { IronMg = 0.1, IronType = "no-hemo" },
                IsInhibitor = false, 
                Category = "BEVERAGE" 
            },
            new FoodItem 
            { 
                FoodId = "FOOD_042", 
                Name = "Agua", 
                NutrientContent = new NutrientContent { IronMg = 0.0, IronType = "no-hemo" },
                IsInhibitor = false, 
                Category = "BEVERAGE" 
            }
        };
    }
}