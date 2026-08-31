using WebApplication1.NutritionDiary.Domain.Models.Entities;

namespace WebApplication1.NutritionDiary.Infrastructure.Mappers;



public static class FoodEntryMapper
{
    public static FoodEntry ToDomain(dynamic document)
    {
        return new FoodEntry(
            document.id,
            document.diaryId,
            document.foodItemId,
            document.quantity,
            document.unit,
            document.ironContributed,
            document.registeredAt
        );
    }

    public static object ToPersistence(FoodEntry entry)
    {
        return entry.ToPrimitives();
    }
}