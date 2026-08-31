using WebApplication1.NutritionDiary.Domain.Models.Aggregate;

namespace WebApplication1.NutritionDiary.Infrastructure.Mappers;

public static class NutritionalDiaryMapper
{
    public static NutritionalDiary ToDomain(dynamic document)
    {
        return new NutritionalDiary(
            document.id,
            document.patientId,
            document.motherId,
            document.date,
            document.totalIronAbsorbed,
            document.hasInhibitor
        );
    }

    public static object ToPersistence(NutritionalDiary diary)
    {
        return diary.ToPrimitives();
    }
}