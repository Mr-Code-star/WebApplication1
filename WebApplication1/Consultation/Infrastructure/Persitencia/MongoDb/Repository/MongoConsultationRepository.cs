using WebApplication1.Consultation.Domain.Repositories;
using WebApplication1.Consultation.Infrastructure.Mappers;
using WebApplication1.Consultation.Infrastructure.Persitencia.MongoDb.Models;

namespace WebApplication1.Consultation.Infrastructure.Persitencia.MongoDb.Repository;

using Microsoft.Extensions.Logging;
using MongoDB.Driver;


public class MongoConsultationRepository : IConsultationRepository
{
    private readonly IMongoCollection<ConsultationDocument> _collection;
    private readonly ILogger<MongoConsultationRepository> _logger;

    public MongoConsultationRepository(IMongoDatabase database, ILogger<MongoConsultationRepository> logger)
    {
        _collection = database.GetCollection<ConsultationDocument>("consultations");
        _logger = logger;
    }

    public async Task SaveAsync(Domain.Models.Aggregate.Consultation consultation)
    {
        var data = ConsultationMapper.ToPersistence(consultation);

        var document = new ConsultationDocument
        {
            ConsultationId = (string)data.GetType().GetProperty("Id")?.GetValue(data, null)!,
            PatientId = (string)data.GetType().GetProperty("PatientId")?.GetValue(data, null)!,
            MotherId = (string)data.GetType().GetProperty("MotherId")?.GetValue(data, null)!,
            NurseId = (string)data.GetType().GetProperty("NurseId")?.GetValue(data, null)!,
            CreatedAt = (DateTime)data.GetType().GetProperty("CreatedAt")?.GetValue(data, null)!,
            ClosedAt = (DateTime?)data.GetType().GetProperty("ClosedAt")?.GetValue(data, null),
            Messages = ((IEnumerable<dynamic>)data.GetType().GetProperty("Messages")?.GetValue(data, null) ?? Enumerable.Empty<dynamic>())
                .Select(m => new MessageDocument
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    SenderRole = m.SenderRole,
                    Content = m.Content,
                    SentAt = m.SentAt
                }).ToList()
        };

        await _collection.InsertOneAsync(document);
        _logger.LogInformation("Consulta creada: {ConsultationId}", document.ConsultationId);
    }

    public async Task UpdateAsync(Domain.Models.Aggregate.Consultation consultation)
    {
        var data = ConsultationMapper.ToPersistence(consultation);
        var consultationId = (string)data.GetType().GetProperty("Id")?.GetValue(data, null)!;

        var filter = Builders<ConsultationDocument>.Filter.Eq(x => x.ConsultationId, consultationId);

        var messages = ((IEnumerable<dynamic>)data.GetType().GetProperty("Messages")?.GetValue(data, null) ?? Enumerable.Empty<dynamic>())
            .Select(m => new MessageDocument
            {
                Id = m.Id,
                SenderId = m.SenderId,
                SenderRole = m.SenderRole,
                Content = m.Content,
                SentAt = m.SentAt
            }).ToList();

        var update = Builders<ConsultationDocument>.Update
            .Set(x => x.Messages, messages)
            .Set(x => x.ClosedAt, (DateTime?)data.GetType().GetProperty("ClosedAt")?.GetValue(data, null))
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        await _collection.UpdateOneAsync(filter, update);
        _logger.LogInformation("Consulta actualizada: {ConsultationId}", consultationId);
    }

    public async Task<Domain.Models.Aggregate.Consultation?> FindByIdAsync(string consultationId)
    {
        var filter = Builders<ConsultationDocument>.Filter.Eq(x => x.ConsultationId, consultationId);
        var document = await _collection.Find(filter).FirstOrDefaultAsync();

        if (document == null) return null;

        return ConsultationMapper.ToDomain(document);
    }

    public async Task<List<Domain.Models.Aggregate.Consultation>> FindOpenByMotherIdAsync(string motherId)
    {
        var filter = Builders<ConsultationDocument>.Filter.Eq(x => x.MotherId, motherId);
        var documents = await _collection.Find(filter).ToListAsync();

        return documents.Select(ConsultationMapper.ToDomain).ToList();
    }

    public async Task<List<Domain.Models.Aggregate.Consultation>> FindOpenByNurseIdAsync(string nurseId)
    {
        var filter = Builders<ConsultationDocument>.Filter.Eq(x => x.NurseId, nurseId);
        var documents = await _collection.Find(filter).ToListAsync();

        return documents.Select(ConsultationMapper.ToDomain).ToList();
    }

    public async Task<Domain.Models.Aggregate.Consultation?> FindOpenByPatientIdAsync(string patientId)
    {
        var filter = Builders<ConsultationDocument>.Filter.Eq(x => x.PatientId, patientId);
        var document = await _collection.Find(filter).FirstOrDefaultAsync();

        if (document == null) return null;

        return ConsultationMapper.ToDomain(document);
    }

    public async Task DeleteAsync(string consultationId)
    {
        var filter = Builders<ConsultationDocument>.Filter.Eq(x => x.ConsultationId, consultationId);
        await _collection.DeleteOneAsync(filter);
        _logger.LogInformation("Consulta eliminada: {ConsultationId}", consultationId);
    }
}