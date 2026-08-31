using WebApplication1.patient_management.Domain.Aggregate;
using WebApplication1.patient_management.Domain.Entities;
using WebApplication1.patient_management.Domain.Enums;
using WebApplication1.patient_management.Domain.Repositories;
using WebApplication1.patient_management.Infrastructure.Mapper;
using WebApplication1.patient_management.Infrastructure.Persitencia.MongoDb.Models;

namespace WebApplication1.patient_management.Infrastructure.Persitencia.MongoDb.Repositoreis;

using Microsoft.Extensions.Logging;
using MongoDB.Driver;


public class MongoPatientRepository : IPatientRepository
{
    private readonly IMongoCollection<PatientDocument> _collection;
    private readonly ILogger<MongoPatientRepository> _logger;

    public MongoPatientRepository(IMongoDatabase database, ILogger<MongoPatientRepository> logger)
    {
        _collection = database.GetCollection<PatientDocument>("patients");
        _logger = logger;
    }

    public async Task<Patient> SaveAsync(Patient patient)
    {
        var data = PatientMapper.ToPersistence(patient);
        var document = new PatientDocument
        {
            PatientId = (string)data.GetType().GetProperty("id")?.GetValue(data, null)!,
            Name = (string)data.GetType().GetProperty("name")?.GetValue(data, null)!,
            LastName = (string)data.GetType().GetProperty("lastName")?.GetValue(data, null)!,
            BirthDate = (DateTime)data.GetType().GetProperty("birthDate")?.GetValue(data, null)!,
            CurrentWeight = (double)data.GetType().GetProperty("currentWeight")?.GetValue(data, null)!,
            CurrentHeight = (double)data.GetType().GetProperty("currentHeight")?.GetValue(data, null)!,
            MotherId = (string)data.GetType().GetProperty("motherId")?.GetValue(data, null)!,
            NurseId = (string?)data.GetType().GetProperty("nurseId")?.GetValue(data, null),
            Gender = (string)data.GetType().GetProperty("gender")?.GetValue(data, null)!,
            FacilityId = (string?)data.GetType().GetProperty("facilityId")?.GetValue(data, null),
            Status = (string)data.GetType().GetProperty("status")?.GetValue(data, null)!,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _collection.InsertOneAsync(document);
        return patient;
    }

    public async Task<Patient?> FindByIdAsync(string patientId)
    {
        var filter = Builders<PatientDocument>.Filter.Eq(x => x.PatientId, patientId);
        var document = await _collection.Find(filter).FirstOrDefaultAsync();

        if (document == null) return null;

        return PatientMapper.ToDomain(document);
    }

    public async Task<IReadOnlyList<Patient>> FindByMotherIdAsync(string motherId)
    {
        var filter = Builders<PatientDocument>.Filter.Eq(x => x.MotherId, motherId);
        var documents = await _collection.Find(filter).ToListAsync();

        return documents.Select(PatientMapper.ToDomain).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<Patient>> FindByNurseIdAsync(string nurseId)
    {
        var filter = Builders<PatientDocument>.Filter.Eq(x => x.NurseId, nurseId);
        var documents = await _collection.Find(filter).ToListAsync();

        return documents.Select(PatientMapper.ToDomain).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<Patient>> FindByStatusAsync(PatientStatus status)
    {
        var filter = Builders<PatientDocument>.Filter.Eq(x => x.Status, status.ToStringValue());
        var documents = await _collection.Find(filter).ToListAsync();

        return documents.Select(PatientMapper.ToDomain).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<Patient>> FindAllAsync()
    {
        var documents = await _collection.Find(_ => true).ToListAsync();
        return documents.Select(PatientMapper.ToDomain).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<Patient>> FindPatientsAssignedToNurseAsync(string nurseId, string? searchTerm = null)
    {
        var filter = Builders<PatientDocument>.Filter.Eq(x => x.NurseId, nurseId);
        var documents = await _collection.Find(filter).ToListAsync();

        var patients = documents.Select(PatientMapper.ToDomain).ToList();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            patients = patients.Where(p =>
                p.Name.ToLower().Contains(term) ||
                p.LastName.ToLower().Contains(term)
            ).ToList();
        }

        return patients.AsReadOnly();
    }

    public async Task<IReadOnlyList<Patient>> FindPatientsEligibleForDischargeAsync(string nurseId, string? searchTerm = null)
    {
        var filter = Builders<PatientDocument>.Filter.And(
            Builders<PatientDocument>.Filter.Eq(x => x.NurseId, nurseId),
            Builders<PatientDocument>.Filter.Ne(x => x.Status, PatientStatus.Discharged.ToStringValue())
        );

        var documents = await _collection.Find(filter).ToListAsync();
        var patients = documents.Select(PatientMapper.ToDomain).ToList();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            patients = patients.Where(p =>
                p.Name.ToLower().Contains(term) ||
                p.LastName.ToLower().Contains(term)
            ).ToList();
        }

        return patients.AsReadOnly();
    }

    public async Task<int> CountActivePatientsByNurseAsync(string nurseId)
    {
        var filter = Builders<PatientDocument>.Filter.And(
            Builders<PatientDocument>.Filter.Eq(x => x.NurseId, nurseId),
            Builders<PatientDocument>.Filter.Ne(x => x.Status, PatientStatus.Discharged.ToStringValue())
        );

        return (int)await _collection.CountDocumentsAsync(filter);
    }

     public async Task<Patient> UpdateAsync(Patient patient)
    {
        try
        {
            var data = PatientMapper.ToPersistence(patient);
            var patientId = (string)data.GetType().GetProperty("id")?.GetValue(data, null)!;

            var filter = Builders<PatientDocument>.Filter.Eq(x => x.PatientId, patientId);

            var update = Builders<PatientDocument>.Update
                .Set(x => x.Name, (string)data.GetType().GetProperty("name")?.GetValue(data, null)!)
                .Set(x => x.LastName, (string)data.GetType().GetProperty("lastName")?.GetValue(data, null)!)
                .Set(x => x.BirthDate, (DateTime)data.GetType().GetProperty("birthDate")?.GetValue(data, null)!)
                .Set(x => x.CurrentWeight, (double)data.GetType().GetProperty("currentWeight")?.GetValue(data, null)!)
                .Set(x => x.CurrentHeight, (double)data.GetType().GetProperty("currentHeight")?.GetValue(data, null)!)
                .Set(x => x.MotherId, (string)data.GetType().GetProperty("motherId")?.GetValue(data, null)!)
                .Set(x => x.NurseId, (string?)data.GetType().GetProperty("nurseId")?.GetValue(data, null))
                .Set(x => x.Gender, (string)data.GetType().GetProperty("gender")?.GetValue(data, null)!)
                .Set(x => x.FacilityId, (string?)data.GetType().GetProperty("facilityId")?.GetValue(data, null))
                .Set(x => x.Status, (string)data.GetType().GetProperty("status")?.GetValue(data, null)!)
                .Set(x => x.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(filter, update);

            if (result.ModifiedCount == 0)
            {
                _logger.LogWarning("No se encontró paciente para actualizar: {PatientId}", patientId);
                throw new Exception($"Patient with id {patientId} not found");
            }

            _logger.LogInformation("Paciente actualizado: {PatientId}", patientId);
            return patient;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar paciente");
            throw;
        }
    }
     
    public async Task<MedicalRecord?> FindMedicalRecordByPatientIdAsync(string patientId)
    {
        // Esto debería usar el repositorio de MedicalRecord
        // Por ahora retornamos null
        return null;
    }

    public async Task<MedicalRecord> SaveMedicalRecordAsync(MedicalRecord medicalRecord)
    {
        // Esto debería usar el repositorio de MedicalRecord
        return medicalRecord;
    }

    public async Task<bool> HasMedicalRecordAsync(string patientId)
    {
        // Esto debería usar el repositorio de MedicalRecord
        return false;
    }

    public async Task<Control> SaveControlAsync(Control control)
    {
        // Esto debería usar el repositorio de MedicalRecord
        return control;
    }

    public async Task<IReadOnlyList<Control>> FindControlsByMedicalRecordIdAsync(string medicalRecordId)
    {
        // Esto debería usar el repositorio de MedicalRecord
        return new List<Control>().AsReadOnly();
    }
}