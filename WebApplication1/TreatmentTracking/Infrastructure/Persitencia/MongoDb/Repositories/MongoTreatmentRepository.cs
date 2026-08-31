using MongoDB.Driver;
using WebApplication1.TreatmentTracking.Domain.Model.Aggregate;
using WebApplication1.TreatmentTracking.Domain.Model.ValueObjects;
using WebApplication1.TreatmentTracking.Domain.Repositories;
using WebApplication1.TreatmentTracking.Infrastructure.Mappers;
using WebApplication1.TreatmentTracking.Infrastructure.Persitencia.MongoDb.Models;

namespace WebApplication1.TreatmentTracking.Infrastructure.Persitencia.MongoDb.Repositories;

public class MongoTreatmentRepository : ITreatmentRepository
{
    private readonly IMongoCollection<TreatmentDocument> _collection;
    private readonly ILogger<MongoTreatmentRepository> _logger;

    public MongoTreatmentRepository(IMongoDatabase database, ILogger<MongoTreatmentRepository> logger)
    {
        _collection = database.GetCollection<TreatmentDocument>("treatments");
        _logger = logger;
    }

    public async Task SaveAsync(Treatment treatment)
    {
        var data = TreatmentMapper.ToPersistence(treatment);

        var document = new TreatmentDocument
        {
            TreatmentId = (string)data.GetType().GetProperty("id")?.GetValue(data, null)!,
            PatientId = (string)data.GetType().GetProperty("patientId")?.GetValue(data, null)!,
            NurseId = (string)data.GetType().GetProperty("nurseId")?.GetValue(data, null)!,
            Supplement = (string)data.GetType().GetProperty("supplement")?.GetValue(data, null)!,
            Quantity = (string)data.GetType().GetProperty("quantity")?.GetValue(data, null)!,
            DosingHours = (string)data.GetType().GetProperty("dosingHours")?.GetValue(data, null)!,
            DurationDays = (int)data.GetType().GetProperty("durationDays")?.GetValue(data, null)!,
            StartDate = (DateTime)data.GetType().GetProperty("startDate")?.GetValue(data, null)!,
            EndDate = (DateTime)data.GetType().GetProperty("endDate")?.GetValue(data, null)!,
            Status = (string)data.GetType().GetProperty("status")?.GetValue(data, null)!,
            AdherenceScore = (double)data.GetType().GetProperty("adherenceScore")?.GetValue(data, null)!,
            CurrentStreak = (int)data.GetType().GetProperty("currentStreak")?.GetValue(data, null)!,
            TotalConfirmed = (int)data.GetType().GetProperty("totalConfirmed")?.GetValue(data, null)!,
            TotalOmitted = (int)data.GetType().GetProperty("totalOmitted")?.GetValue(data, null)!,
            CompletionObservation = (string?)data.GetType().GetProperty("completionObservation")?.GetValue(data, null),
            AbandonmentObservation = (string?)data.GetType().GetProperty("abandonmentObservation")?.GetValue(data, null),
            RiskScore = new RiskScoreDocument
            {
                Id = (string)data.GetType().GetProperty("riskScore")?.GetType().GetProperty("id")?.GetValue(data.GetType().GetProperty("riskScore")?.GetValue(data, null), null)!,
                Score = (int)data.GetType().GetProperty("riskScore")?.GetType().GetProperty("score")?.GetValue(data.GetType().GetProperty("riskScore")?.GetValue(data, null), null)!,
                RiskLevel = (string)data.GetType().GetProperty("riskScore")?.GetType().GetProperty("riskLevel")?.GetValue(data.GetType().GetProperty("riskScore")?.GetValue(data, null), null)!,
                CalculatedAt = (DateTime)data.GetType().GetProperty("riskScore")?.GetType().GetProperty("calculatedAt")?.GetValue(data.GetType().GetProperty("riskScore")?.GetValue(data, null), null)!
            }
        };

        await _collection.InsertOneAsync(document);
        _logger.LogInformation("Tratamiento guardado: {TreatmentId}", document.TreatmentId);
    }

    public async Task<Treatment?> FindActiveByPatientIdAsync(string patientId)
    {
        var filter = Builders<TreatmentDocument>.Filter.And(
            Builders<TreatmentDocument>.Filter.Eq(x => x.PatientId, patientId),
            Builders<TreatmentDocument>.Filter.Eq(x => x.Status, TreatmentStatus.ACTIVE.ToStringValue())
        );

        var document = await _collection.Find(filter).FirstOrDefaultAsync();

        if (document == null) return null;

        return TreatmentMapper.ToDomain(document);
    }

    public async Task<List<Treatment>> FindAllActiveAsync()
    {
        var filter = Builders<TreatmentDocument>.Filter.Eq(x => x.Status, TreatmentStatus.ACTIVE.ToStringValue());
        var documents = await _collection.Find(filter).ToListAsync();

        return documents.Select(TreatmentMapper.ToDomain).ToList();
    }

    public async Task<Treatment?> FindByIdAsync(string treatmentId)
    {
        var filter = Builders<TreatmentDocument>.Filter.Eq(x => x.TreatmentId, treatmentId);
        var document = await _collection.Find(filter).FirstOrDefaultAsync();

        if (document == null) return null;

        return TreatmentMapper.ToDomain(document);
    }

    public async Task<List<Treatment>> FindByNurseIdAsync(string nurseId, TreatmentStatus? status = null)
    {
        var filterBuilder = Builders<TreatmentDocument>.Filter;
        var filter = filterBuilder.Eq(x => x.NurseId, nurseId);

        if (status.HasValue)
        {
            filter = filterBuilder.And(filter, filterBuilder.Eq(x => x.Status, status.Value.ToStringValue()));
        }

        var documents = await _collection.Find(filter).ToListAsync();

        return documents.Select(TreatmentMapper.ToDomain).ToList();
    }

    public async Task<List<Treatment>> FindByPatientIdAsync(string patientId)
    {
        var filter = Builders<TreatmentDocument>.Filter.Eq(x => x.PatientId, patientId);
        var documents = await _collection.Find(filter).ToListAsync();

        return documents.Select(TreatmentMapper.ToDomain).ToList();
    }

    public async Task<List<Treatment>> FindByRiskLevelAsync(RiskLevel riskLevel, string? nurseId = null)
    {
        var filterBuilder = Builders<TreatmentDocument>.Filter;
        var filter = filterBuilder.And(
            filterBuilder.Eq(x => x.Status, TreatmentStatus.ACTIVE.ToStringValue()),
            filterBuilder.Eq(x => x.RiskScore.RiskLevel, riskLevel.ToStringValue())
        );

        if (!string.IsNullOrEmpty(nurseId))
        {
            filter = filterBuilder.And(filter, filterBuilder.Eq(x => x.NurseId, nurseId));
        }

        var documents = await _collection.Find(filter).ToListAsync();

        return documents.Select(TreatmentMapper.ToDomain).ToList();
    }

    public async Task UpdateAsync(Treatment treatment)
    {
        var data = TreatmentMapper.ToPersistence(treatment);
        var treatmentId = (string)data.GetType().GetProperty("id")?.GetValue(data, null)!;

        var filter = Builders<TreatmentDocument>.Filter.Eq(x => x.TreatmentId, treatmentId);

        var update = Builders<TreatmentDocument>.Update
            .Set(x => x.Status, (string)data.GetType().GetProperty("status")?.GetValue(data, null)!)
            .Set(x => x.AdherenceScore, (double)data.GetType().GetProperty("adherenceScore")?.GetValue(data, null)!)
            .Set(x => x.CurrentStreak, (int)data.GetType().GetProperty("currentStreak")?.GetValue(data, null)!)
            .Set(x => x.TotalConfirmed, (int)data.GetType().GetProperty("totalConfirmed")?.GetValue(data, null)!)
            .Set(x => x.TotalOmitted, (int)data.GetType().GetProperty("totalOmitted")?.GetValue(data, null)!)
            .Set(x => x.CompletionObservation, (string?)data.GetType().GetProperty("completionObservation")?.GetValue(data, null))
            .Set(x => x.AbandonmentObservation, (string?)data.GetType().GetProperty("abandonmentObservation")?.GetValue(data, null))
            .Set(x => x.RiskScore, new RiskScoreDocument
            {
                Id = (string)data.GetType().GetProperty("riskScore")?.GetType().GetProperty("id")?.GetValue(data.GetType().GetProperty("riskScore")?.GetValue(data, null), null)!,
                Score = (int)data.GetType().GetProperty("riskScore")?.GetType().GetProperty("score")?.GetValue(data.GetType().GetProperty("riskScore")?.GetValue(data, null), null)!,
                RiskLevel = (string)data.GetType().GetProperty("riskScore")?.GetType().GetProperty("riskLevel")?.GetValue(data.GetType().GetProperty("riskScore")?.GetValue(data, null), null)!,
                CalculatedAt = (DateTime)data.GetType().GetProperty("riskScore")?.GetType().GetProperty("calculatedAt")?.GetValue(data.GetType().GetProperty("riskScore")?.GetValue(data, null), null)!
            });

        await _collection.UpdateOneAsync(filter, update);
        _logger.LogInformation("Tratamiento actualizado: {TreatmentId}", treatmentId);
    }

    public async Task DeleteAsync(string treatmentId)
    {
        var filter = Builders<TreatmentDocument>.Filter.Eq(x => x.TreatmentId, treatmentId);
        await _collection.DeleteOneAsync(filter);
        _logger.LogInformation("Tratamiento eliminado: {TreatmentId}", treatmentId);
    }
}