using MongoDB.Driver;
using WebApplication1.AchievementsRewards.Domain.Model.Aggregate;
using WebApplication1.AchievementsRewards.Domain.Model.ValueObjects;
using WebApplication1.AchievementsRewards.Domain.Repositories;
using WebApplication1.AchievementsRewards.Infrastructure.Mappers;
using WebApplication1.AchievementsRewards.Infrastructure.Persitencia.MongoDb.Models;

namespace WebApplication1.AchievementsRewards.Infrastructure.Persitencia.MongoDb.Repository;


public class MongoAchievementRepository : IAchievementRepository
{
    private readonly IMongoCollection<AchievementDocument> _collection;
    private readonly ILogger<MongoAchievementRepository> _logger;

    public MongoAchievementRepository(IMongoDatabase database, ILogger<MongoAchievementRepository> logger)
    {
        _collection = database.GetCollection<AchievementDocument>("achievements");
        _logger = logger;
    }

    public async Task SaveAsync(Achievement achievement)
    {
        var data = AchievementMapper.ToPersistence(achievement);
        
        var document = new AchievementDocument
        {
            AchievementId = (string)data.GetType().GetProperty("id")?.GetValue(data, null)!,
            PatientId = (string)data.GetType().GetProperty("patientId")?.GetValue(data, null)!,
            MotherId = (string)data.GetType().GetProperty("motherId")?.GetValue(data, null)!,
            TreatmentId = (string)data.GetType().GetProperty("treatmentId")?.GetValue(data, null)!,
            DurationDays = (int)data.GetType().GetProperty("durationDays")?.GetValue(data, null)!,
            CurrentStreak = (int)data.GetType().GetProperty("currentStreak")?.GetValue(data, null)!,
            LongestStreak = (int)data.GetType().GetProperty("longestStreak")?.GetValue(data, null)!,
            BestStreak = (int)data.GetType().GetProperty("bestStreak")?.GetValue(data, null)!,
            StreakStartDate = (DateTime?)data.GetType().GetProperty("streakStartDate")?.GetValue(data, null),
            TotalPoints = (int)data.GetType().GetProperty("totalPoints")?.GetValue(data, null)!,
            Status = (string)data.GetType().GetProperty("status")?.GetValue(data, null)!,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _collection.InsertOneAsync(document);
        _logger.LogInformation("Achievement guardado: {AchievementId}", document.AchievementId);
    }

    public async Task UpdateAsync(Achievement achievement)
    {
        var data = AchievementMapper.ToPersistence(achievement);
        var achievementId = (string)data.GetType().GetProperty("id")?.GetValue(data, null)!;

        var filter = Builders<AchievementDocument>.Filter.Eq(x => x.AchievementId, achievementId);

        var update = Builders<AchievementDocument>.Update
            .Set(x => x.PatientId, (string)data.GetType().GetProperty("patientId")?.GetValue(data, null)!)
            .Set(x => x.MotherId, (string)data.GetType().GetProperty("motherId")?.GetValue(data, null)!)
            .Set(x => x.TreatmentId, (string)data.GetType().GetProperty("treatmentId")?.GetValue(data, null)!)
            .Set(x => x.DurationDays, (int)data.GetType().GetProperty("durationDays")?.GetValue(data, null)!)
            .Set(x => x.CurrentStreak, (int)data.GetType().GetProperty("currentStreak")?.GetValue(data, null)!)
            .Set(x => x.LongestStreak, (int)data.GetType().GetProperty("longestStreak")?.GetValue(data, null)!)
            .Set(x => x.BestStreak, (int)data.GetType().GetProperty("bestStreak")?.GetValue(data, null)!)
            .Set(x => x.StreakStartDate, (DateTime?)data.GetType().GetProperty("streakStartDate")?.GetValue(data, null))
            .Set(x => x.TotalPoints, (int)data.GetType().GetProperty("totalPoints")?.GetValue(data, null)!)
            .Set(x => x.Status, (string)data.GetType().GetProperty("status")?.GetValue(data, null)!)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        await _collection.UpdateOneAsync(filter, update);
        _logger.LogInformation("Achievement actualizado: {AchievementId}", achievementId);
    }

    public async Task<Achievement?> FindByIdAsync(string id)
    {
        var filter = Builders<AchievementDocument>.Filter.Eq(x => x.AchievementId, id);
        var document = await _collection.Find(filter).FirstOrDefaultAsync();

        if (document == null) return null;

        return AchievementMapper.ToDomain(document);
    }

    public async Task<Achievement?> FindByTreatmentIdAsync(string treatmentId)
    {
        var filter = Builders<AchievementDocument>.Filter.Eq(x => x.TreatmentId, treatmentId);
        var document = await _collection.Find(filter).FirstOrDefaultAsync();

        if (document == null) return null;

        return AchievementMapper.ToDomain(document);
    }

    public async Task<Achievement?> FindByPatientIdAsync(string patientId)
    {
        var filter = Builders<AchievementDocument>.Filter.Eq(x => x.PatientId, patientId);
        var document = await _collection.Find(filter).FirstOrDefaultAsync();

        if (document == null) return null;

        return AchievementMapper.ToDomain(document);
    }

    public async Task<List<Achievement>> FindByMotherIdAsync(string motherId)
    {
        var filter = Builders<AchievementDocument>.Filter.Eq(x => x.MotherId, motherId);
        var documents = await _collection.Find(filter).ToListAsync();

        return documents.Select(AchievementMapper.ToDomain).ToList();
    }

    public async Task<List<Achievement>> FindAllActiveAsync()
    {
        var filter = Builders<AchievementDocument>.Filter.Eq(x => x.Status, AchievementStatus.ACTIVE.ToStringValue());
        var documents = await _collection.Find(filter).ToListAsync();

        return documents.Select(AchievementMapper.ToDomain).ToList();
    }

    public async Task DeleteAsync(string id)
    {
        var filter = Builders<AchievementDocument>.Filter.Eq(x => x.AchievementId, id);
        await _collection.DeleteOneAsync(filter);
        _logger.LogInformation("Achievement eliminado: {AchievementId}", id);
    }
}