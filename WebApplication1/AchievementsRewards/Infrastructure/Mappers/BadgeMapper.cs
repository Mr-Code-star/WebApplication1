using WebApplication1.AchievementsRewards.Domain.Model.Entities;
using WebApplication1.AchievementsRewards.Domain.Model.ValueObjects;

namespace WebApplication1.AchievementsRewards.Infrastructure.Mappers;


public static class BadgeMapper
{
    public static Badge ToDomain(dynamic document)
    {
        return new Badge(
            document.id,
            document.achievementId,
            BadgeTypeExtensions.FromString(document.type),
            document.name,
            document.description,
            document.milestone,
            document.isUnlocked,
            document.unlockedAt
        );
    }

    public static object ToPersistence(Badge badge)
    {
        var data = badge.ToPrimitives();

        return new
        {
            id = data.Id,
            achievementId = data.AchievementId,
            type = data.Type,
            name = data.Name,
            description = data.Description,
            milestone = data.Milestone,
            isUnlocked = data.IsUnlocked,
            unlockedAt = data.UnlockedAt
        };
    }
}