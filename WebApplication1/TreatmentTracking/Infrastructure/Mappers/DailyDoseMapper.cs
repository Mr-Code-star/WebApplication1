using WebApplication1.TreatmentTracking.Domain.Model.Entities;
using WebApplication1.TreatmentTracking.Domain.Model.ValueObjects;

namespace WebApplication1.TreatmentTracking.Infrastructure.Mappers;

public static class DailyDoseMapper
{
    public static DailyDose ToDomain(dynamic document)
    {
        return new DailyDose(
            document.id,
            document.treatmentId,
            document.scheduledDate,
            document.confirmedAt,
            DoseStatusExtensions.FromString(document.status)
        );
    }

    public static object ToPersistence(DailyDose dose)
    {
        return dose.ToPrimitives();
    }
}