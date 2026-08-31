using WebApplication1.AchievementsRewards.Domain.Model.Entities;
using WebApplication1.AchievementsRewards.Domain.Model.ValueObjects;
using WebApplication1.AchievementsRewards.Domain.Repositories;
using WebApplication1.AchievementsRewards.Infrastructure.Mappers;
using WebApplication1.AchievementsRewards.Infrastructure.Persitencia.MongoDb.Models;

namespace WebApplication1.AchievementsRewards.Infrastructure.Persitencia.MongoDb.Repository;

using Microsoft.Extensions.Logging;
using MongoDB.Driver;

public class MongoBadgeRepository : IBadgeRepository
{
    private readonly IMongoCollection<BadgeDocument> _collection;
    private readonly ILogger<MongoBadgeRepository> _logger;

    public MongoBadgeRepository(IMongoDatabase database, ILogger<MongoBadgeRepository> logger)
    {
        _collection = database.GetCollection<BadgeDocument>("badges");
        _logger = logger;
    }

    public async Task SaveAsync(Badge badge)
    {
        var data = BadgeMapper.ToPersistence(badge);
        
        var document = new BadgeDocument
        {
            BadgeId = (string)data.GetType().GetProperty("id")?.GetValue(data, null)!,
            AchievementId = (string)data.GetType().GetProperty("achievementId")?.GetValue(data, null)!,
            Type = (string)data.GetType().GetProperty("type")?.GetValue(data, null)!,
            Name = (string)data.GetType().GetProperty("name")?.GetValue(data, null)!,
            Description = (string)data.GetType().GetProperty("description")?.GetValue(data, null)!,
            Milestone = (int)data.GetType().GetProperty("milestone")?.GetValue(data, null)!,
            IsUnlocked = (bool)data.GetType().GetProperty("isUnlocked")?.GetValue(data, null)!,
            UnlockedAt = (DateTime?)data.GetType().GetProperty("unlockedAt")?.GetValue(data, null),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _collection.InsertOneAsync(document);
        _logger.LogInformation("Badge guardado: {BadgeId}", document.BadgeId);
    }

    public async Task SaveManyAsync(List<Badge> badges)
    {
        var documents = badges.Select(b =>
        {
            var data = BadgeMapper.ToPersistence(b);
            return new BadgeDocument
            {
                BadgeId = (string)data.GetType().GetProperty("id")?.GetValue(data, null)!,
                AchievementId = (string)data.GetType().GetProperty("achievementId")?.GetValue(data, null)!,
                Type = (string)data.GetType().GetProperty("type")?.GetValue(data, null)!,
                Name = (string)data.GetType().GetProperty("name")?.GetValue(data, null)!,
                Description = (string)data.GetType().GetProperty("description")?.GetValue(data, null)!,
                Milestone = (int)data.GetType().GetProperty("milestone")?.GetValue(data, null)!,
                IsUnlocked = (bool)data.GetType().GetProperty("isUnlocked")?.GetValue(data, null)!,
                UnlockedAt = (DateTime?)data.GetType().GetProperty("unlockedAt")?.GetValue(data, null),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }).ToList();

        await _collection.InsertManyAsync(documents);
        _logger.LogInformation("{Count} badges guardados", documents.Count);
    }

    public async Task UpdateAsync(Badge badge)
    {
        var data = BadgeMapper.ToPersistence(badge);
        var badgeId = (string)data.GetType().GetProperty("id")?.GetValue(data, null)!;

        var filter = Builders<BadgeDocument>.Filter.Eq(x => x.BadgeId, badgeId);

        var update = Builders<BadgeDocument>.Update
            .Set(x => x.AchievementId, (string)data.GetType().GetProperty("achievementId")?.GetValue(data, null)!)
            .Set(x => x.Type, (string)data.GetType().GetProperty("type")?.GetValue(data, null)!)
            .Set(x => x.Name, (string)data.GetType().GetProperty("name")?.GetValue(data, null)!)
            .Set(x => x.Description, (string)data.GetType().GetProperty("description")?.GetValue(data, null)!)
            .Set(x => x.Milestone, (int)data.GetType().GetProperty("milestone")?.GetValue(data, null)!)
            .Set(x => x.IsUnlocked, (bool)data.GetType().GetProperty("isUnlocked")?.GetValue(data, null)!)
            .Set(x => x.UnlockedAt, (DateTime?)data.GetType().GetProperty("unlockedAt")?.GetValue(data, null))
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        await _collection.UpdateOneAsync(filter, update);
        _logger.LogInformation("Badge actualizado: {BadgeId}", badgeId);
    }

    public async Task UpdateManyAsync(List<Badge> badges)
    {
        var bulkOps = badges.Select(badge =>
        {
            var data = BadgeMapper.ToPersistence(badge);
            var badgeId = (string)data.GetType().GetProperty("id")?.GetValue(data, null)!;

            var update = Builders<BadgeDocument>.Update
                .Set(x => x.IsUnlocked, (bool)data.GetType().GetProperty("isUnlocked")?.GetValue(data, null)!)
                .Set(x => x.UnlockedAt, (DateTime?)data.GetType().GetProperty("unlockedAt")?.GetValue(data, null))
                .Set(x => x.UpdatedAt, DateTime.UtcNow);

            return new UpdateOneModel<BadgeDocument>(
                Builders<BadgeDocument>.Filter.Eq(x => x.BadgeId, badgeId),
                update
            );
        }).ToList();

        await _collection.BulkWriteAsync(bulkOps);
        _logger.LogInformation("{Count} badges actualizados", badges.Count);
    }

    public async Task<Badge?> FindByIdAsync(string id)
    {
        var filter = Builders<BadgeDocument>.Filter.Eq(x => x.BadgeId, id);
        var document = await _collection.Find(filter).FirstOrDefaultAsync();

        if (document == null) return null;

        return BadgeMapper.ToDomain(document);
    }

    public async Task<List<Badge>> FindByAchievementIdAsync(string achievementId)
    {
        var filter = Builders<BadgeDocument>.Filter.Eq(x => x.AchievementId, achievementId);
        var documents = await _collection.Find(filter).ToListAsync();

        return documents.Select(BadgeMapper.ToDomain).ToList();
    }

    public async Task<Badge?> FindByAchievementIdAndTypeAsync(string achievementId, BadgeType type)
    {
        var filter = Builders<BadgeDocument>.Filter.And(
            Builders<BadgeDocument>.Filter.Eq(x => x.AchievementId, achievementId),
            Builders<BadgeDocument>.Filter.Eq(x => x.Type, type.ToStringValue())
        );

        var document = await _collection.Find(filter).FirstOrDefaultAsync();

        if (document == null) return null;

        return BadgeMapper.ToDomain(document);
    }

    public async Task DeleteByAchievementIdAsync(string achievementId)
    {
        var filter = Builders<BadgeDocument>.Filter.Eq(x => x.AchievementId, achievementId);
        await _collection.DeleteManyAsync(filter);
        _logger.LogInformation("Badges eliminados para achievement: {AchievementId}", achievementId);
    }
}