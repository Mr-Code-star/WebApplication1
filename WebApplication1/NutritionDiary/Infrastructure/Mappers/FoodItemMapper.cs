using WebApplication1.NutritionDiary.Domain.Models.Entities;
using WebApplication1.NutritionDiary.Domain.Models.ValueObjects;

namespace WebApplication1.NutritionDiary.Infrastructure.Mappers;



public static class FoodItemMapper
{
    public static FoodItem ToDomain(dynamic document)
    {
        return new FoodItem(
            document.id,
            document.name,
            new NutrientContent(
                document.nutrientContent.ironMg,
                document.nutrientContent.ironType
            ),
            document.isInhibitor,
            FoodCategoryExtensions.FromString(document.category)
        );
    }

    public static object ToPersistence(FoodItem foodItem)
    {
        return foodItem.ToPrimitives();
    }
}