using WebApplication1.patient_management.Domain.Entities;
using WebApplication1.patient_management.Domain.Repositories;
using WebApplication1.patient_management.Infrastructure.Mapper;
using WebApplication1.patient_management.Infrastructure.Persitencia.MongoDb.Models;

namespace WebApplication1.patient_management.Infrastructure.Persitencia.MongoDb.Repositoreis;

using Microsoft.Extensions.Logging;
using MongoDB.Driver;


public class MongoMedicalRecordRepository : IMedicalRecordRepository
{
    private readonly IMongoCollection<MedicalRecordDocument> _collection;
    private readonly ILogger<MongoMedicalRecordRepository> _logger;

    public MongoMedicalRecordRepository(IMongoDatabase database, ILogger<MongoMedicalRecordRepository> logger)
    {
        _collection = database.GetCollection<MedicalRecordDocument>("medicalrecords");
        _logger = logger;
    }

    public async Task<MedicalRecord> SaveAsync(MedicalRecord medicalRecord)
    {
        var data = MedicalRecordMapper.ToPersistence(medicalRecord);
        var document = new MedicalRecordDocument
        {
            MedicalRecordId = (string)data.GetType().GetProperty("id")?.GetValue(data, null)!,
            PatientId = (string)data.GetType().GetProperty("patientId")?.GetValue(data, null)!,
            NurseId = (string?)data.GetType().GetProperty("nurseId")?.GetValue(data, null),
            CreatedAt = (DateTime)data.GetType().GetProperty("createdAt")?.GetValue(data, null)!,
            UpdatedAt = (DateTime)data.GetType().GetProperty("updatedAt")?.GetValue(data, null)!,
            HemoglobinLevel = (double?)data.GetType().GetProperty("hemoglobinLevel")?.GetValue(data, null),
            Weight = (double)data.GetType().GetProperty("weight")?.GetValue(data, null)!,
            Height = (double)data.GetType().GetProperty("height")?.GetValue(data, null)!,
            Gender = (string)data.GetType().GetProperty("gender")?.GetValue(data, null)!,
            Antecedentes = ((IEnumerable<dynamic>)data.GetType().GetProperty("antecedentes")?.GetValue(data, null) ?? Enumerable.Empty<dynamic>())
                .Select(a => new AntecedenteDocument { Type = a.Type, Description = a.Description }).ToList(),
            MotivoConsulta = (string)data.GetType().GetProperty("motivoConsulta")?.GetValue(data, null)!,
            Observaciones = (string?)data.GetType().GetProperty("observaciones")?.GetValue(data, null),
            Sintomas = ((IEnumerable<dynamic>)data.GetType().GetProperty("sintomas")?.GetValue(data, null) ?? Enumerable.Empty<dynamic>())
                .Select(s => (string)s).ToList(),
            Controls = ((IEnumerable<dynamic>)data.GetType().GetProperty("controls")?.GetValue(data, null) ?? Enumerable.Empty<dynamic>())
                .Select(c => new ControlDocument
                {
                    Id = c.Id,
                    Date = c.Date,
                    HemoglobinLevel = c.HemoglobinLevel,
                    AnemiaStatus = c.AnemiaStatus
                }).ToList()
        };

        await _collection.InsertOneAsync(document);
        return medicalRecord;
    }

    public async Task<MedicalRecord?> FindByIdAsync(string medicalRecordId)
    {
        var filter = Builders<MedicalRecordDocument>.Filter.Eq(x => x.MedicalRecordId, medicalRecordId);
        var document = await _collection.Find(filter).FirstOrDefaultAsync();

        if (document == null) return null;

        return MedicalRecordMapper.ToDomain(document);
    }

    public async Task<MedicalRecord?> FindByPatientIdAsync(string patientId)
    {
        var filter = Builders<MedicalRecordDocument>.Filter.Eq(x => x.PatientId, patientId);
        var document = await _collection.Find(filter).FirstOrDefaultAsync();

        if (document == null) return null;

        return MedicalRecordMapper.ToDomain(document);
    }

    public async Task UpdateAsync(MedicalRecord medicalRecord)
    {
        var data = MedicalRecordMapper.ToPersistence(medicalRecord);
        var id = (string)data.GetType().GetProperty("id")?.GetValue(data, null)!;

        var filter = Builders<MedicalRecordDocument>.Filter.Eq(x => x.MedicalRecordId, id);

        var update = Builders<MedicalRecordDocument>.Update
            .Set(x => x.UpdatedAt, DateTime.UtcNow)
            .Set(x => x.Weight, (double)data.GetType().GetProperty("weight")?.GetValue(data, null)!)
            .Set(x => x.Height, (double)data.GetType().GetProperty("height")?.GetValue(data, null)!)
            .Set(x => x.Gender, (string)data.GetType().GetProperty("gender")?.GetValue(data, null)!)
            .Set(x => x.MotivoConsulta, (string)data.GetType().GetProperty("motivoConsulta")?.GetValue(data, null)!)
            .Set(x => x.Observaciones, (string?)data.GetType().GetProperty("observaciones")?.GetValue(data, null));

        await _collection.UpdateOneAsync(filter, update);
    }

    public async Task DeleteAsync(string medicalRecordId)
    {
        var filter = Builders<MedicalRecordDocument>.Filter.Eq(x => x.MedicalRecordId, medicalRecordId);
        await _collection.DeleteOneAsync(filter);
    }
}