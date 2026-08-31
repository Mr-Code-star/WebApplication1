using WebApplication1.AchievementsRewards.Domain.Model.Aggregate;
using WebApplication1.AchievementsRewards.Domain.Model.ValueObjects;

namespace WebApplication1.AchievementsRewards.Infrastructure.Mappers;



public static class AchievementMapper
{
    public static Achievement ToDomain(dynamic document)
    {
        return new Achievement(
            document.id,
            document.patientId,
            document.motherId,
            document.treatmentId,
            document.durationDays,
            document.currentStreak,
            document.longestStreak,
            document.bestStreak,
            document.streakStartDate,
            document.totalPoints,
            AchievementStatusExtensions.FromString(document.status)
        );
    }

    public static object ToPersistence(Achievement achievement)
    {
        var data = achievement.ToPrimitives();

        return new
        {
            id = data.Id,
            patientId = data.PatientId,
            motherId = data.MotherId,
            treatmentId = data.TreatmentId,
            durationDays = data.DurationDays,
            currentStreak = data.CurrentStreak,
            longestStreak = data.LongestStreak,
            bestStreak = data.BestStreak,
            streakStartDate = data.StreakStartDate,
            totalPoints = data.TotalPoints,
            status = data.Status
        };
    }
}