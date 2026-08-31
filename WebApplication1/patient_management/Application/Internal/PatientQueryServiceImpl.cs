using WebApplication1.Contexts.IAM.Domain.Repositories;
using WebApplication1.patient_management.Domain.Aggregate;
using WebApplication1.patient_management.Domain.Entities;
using WebApplication1.patient_management.Domain.Queries;
using WebApplication1.patient_management.Domain.Repositories;
using WebApplication1.patient_management.Domain.Services;
using WebApplication1.patient_management.Infrastructure.Services;

namespace WebApplication1.patient_management.Application.Internal;

public class PatientQueryServiceImpl : IPatientQueryService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly IUserRepository _userRepository;

    public PatientQueryServiceImpl(
        IPatientRepository patientRepository,
        IMedicalRecordRepository medicalRecordRepository,
        IUserRepository userRepository)
    {
        _patientRepository = patientRepository;
        _medicalRecordRepository = medicalRecordRepository;
        _userRepository = userRepository;
    }

    // ==========================================
    // FILTROS PRIVADOS
    // ==========================================

    private List<T> FilterPatientsBySearchTerm<T>(List<T> items, string? searchTerm) where T : IPatientWithName
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return items;

        var terms = searchTerm.Trim().ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        return items.Where(item =>
        {
            var fullName = $"{item.Name} {item.LastName}".ToLower();
            var nameLower = item.Name.ToLower();
            var lastNameLower = item.LastName.ToLower();

            if (terms.Length == 1)
            {
                var term = terms[0];
                return nameLower.Contains(term) ||
                       lastNameLower.Contains(term) ||
                       fullName.Contains(term);
            }

            return terms.All(term =>
                nameLower.Contains(term) ||
                lastNameLower.Contains(term) ||
                fullName.Contains(term));
        }).ToList();
    }

    // ==========================================
    // IMPLEMENTACIÓN DE LA INTERFAZ
    // ==========================================

    public async Task<Patient?> GetPatientBasicInfoAsync(GetPatientBasicInfoQuery query)
    {
        return await _patientRepository.FindByIdAsync(query.PatientId);
    }

    public async Task<object> GetMotherPatientsSummaryAsync(GetMotherPatientsSummaryQuery query)
    {
        var patients = await _patientRepository.FindByMotherIdAsync(query.MotherId);

        return patients.Select(patient =>
        {
            var data = patient.ToPrimitives();
            return new
            {
                id = data.Id,
                name = data.Name
            };
        }).ToList();
    }

    public async Task<int> GetActivePatientsCountAsync(GetActivePatientsCountQuery query)
    {
        var patients = await _patientRepository.FindByNurseIdAsync(query.NurseId);
        return patients.Count(p => p.ToPrimitives().Status != "DISCHARGED");
    }

    public async Task<byte[]> DownloadHemoglobinReportPdfAsync(DownloadHemoglobinReportPdfQuery query)
    {
        var medicalRecord = await _medicalRecordRepository.FindByIdAsync(query.MedicalRecordId);

        if (medicalRecord == null)
        {
            throw new Exception("Medical record not found");
        }

        return await PdfService.GenerateHemoglobinReportPdfAsync(medicalRecord.ToPrimitives());
    }

    public async Task<byte[]> DownloadMedicalRecordPdfAsync(DownloadMedicalRecordPdfQuery query)
    {
        var medicalRecord = await _medicalRecordRepository.FindByIdAsync(query.MedicalRecordId);

        if (medicalRecord == null)
        {
            throw new Exception("Medical record not found");
        }

        var medicalData = medicalRecord.ToPrimitives();
        var patient = await _patientRepository.FindByIdAsync(medicalData.PatientId);

        if (patient == null)
        {
            throw new Exception("Patient not found");
        }

        return await PdfService.GenerateMedicalRecordPdfAsync(patient.ToPrimitives(), medicalData);
    }

    public async Task<IReadOnlyList<Control>> GetHemoglobinControlsHistoryAsync(GetHemoglobinControlsHistoryQuery query)
    {
        var medicalRecord = await _medicalRecordRepository.FindByIdAsync(query.MedicalRecordId);

        if (medicalRecord == null)
        {
            throw new Exception("Medical record not found");
        }

        return medicalRecord.Controls.AsReadOnly();
    }

    public async Task<MedicalRecord?> GetMedicalRecordAsync(GetMedicalRecordQuery query)
    {
        return await _medicalRecordRepository.FindByPatientIdAsync(query.PatientId);
    }

    public async Task<IReadOnlyList<Patient>> GetPatientsEligibleForDischargeAsync(GetPatientsEligibleForDischargeQuery query)
    {
        var patients = await _patientRepository.FindPatientsEligibleForDischargeAsync(query.NurseId);

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var patientList = patients.ToList();
            var filtered = FilterPatientsBySearchTerm(patientList, query.SearchTerm);
            return filtered.AsReadOnly();
        }

        return patients;
    }

    public async Task<IReadOnlyList<Patient>> ListPatientsByMotherAsync(ListPatientsByMotherQuery query)
    {
        return await _patientRepository.FindByMotherIdAsync(query.MotherId);
    }

    public async Task<object> SearchMotherByDniAsync(SearchMotherByDniQuery query)
    {
        var searchTerm = query.SearchTerm;

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            throw new Exception("Search term is required");
        }

        var mothers = await _userRepository.FindMothersBySearchTermAsync(searchTerm);

        if (mothers == null || mothers.Count == 0)
        {
            throw new Exception("No mothers found matching the search criteria");
        }

        return mothers.Select(m => new
        {
            motherId = m.Id.Value,
            fullName = $"{m.Name} {m.Lastname}".Trim(),
            dni = m.Dni.Value
        }).ToList();
    }

    public async Task<IReadOnlyList<Patient>> GetPatientsAssignedToNurseAsync(GetPatientsAssignedToNurseQuery query)
    {
        var patients = await _patientRepository.FindByNurseIdAsync(query.NurseId);

        var activePatients = patients.Where(p => p.ToPrimitives().Status != "DISCHARGED").ToList();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var filtered = FilterPatientsBySearchTerm(activePatients, query.SearchTerm);
            return filtered.AsReadOnly();
        }

        return activePatients.AsReadOnly();
    }

    public async Task<object> GetHemoglobinEvolutionChartAsync(GetHemoglobinEvolutionChartQuery query)
    {
        var medicalRecord = await _medicalRecordRepository.FindByPatientIdAsync(query.PatientId);

        if (medicalRecord == null)
        {
            throw new Exception("Medical record not found");
        }

        var controls = medicalRecord.Controls.OrderBy(c => c.Date).ToList();

        var chartData = controls.Select(c => new
        {
            date = c.Date,
            hemoglobinLevel = c.HemoglobinLevel.Value
        }).ToList();

        double? latestValue = controls.Count > 0 ? controls.Last().HemoglobinLevel.Value : null;

        return new
        {
            currentHemoglobin = latestValue,
            chart = chartData
        };
    }

    public async Task<Patient?> GetPatientAsync(GetPatientQuery query)
    {
        return await _patientRepository.FindByIdAsync(query.PatientId);
    }

    public async Task<MedicalRecord?> GetMedicalRecordByIdAsync(string medicalRecordId)
    {
        return await _medicalRecordRepository.FindByIdAsync(medicalRecordId);
    }

    public async Task<bool> CheckPatientMedicalRecordAsync(CheckPatientMedicalRecordQuery query)
    {
        var medicalRecord = await _medicalRecordRepository.FindByPatientIdAsync(query.PatientId);
        return medicalRecord != null;
    }
}

// ==========================================
// INTERFAZ PARA FILTROS
// ==========================================

public interface IPatientWithName
{
    string Name { get; }
    string LastName { get; }
}