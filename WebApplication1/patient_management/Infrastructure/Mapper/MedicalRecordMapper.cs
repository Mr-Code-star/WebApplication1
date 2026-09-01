using WebApplication1.patient_management.Domain.Entities;
using WebApplication1.patient_management.Domain.Enums;
using WebApplication1.patient_management.Domain.ValueObjects;
using WebApplication1.patient_management.Infrastructure.Persitencia.MongoDb.Models;

namespace WebApplication1.patient_management.Infrastructure.Mapper;

public static class MedicalRecordMapper
{
    public static MedicalRecord ToDomain(MedicalRecordDocument document)
    {
        if (document == null)
            throw new ArgumentNullException(nameof(document));

        // Convertir controles
        var controls = (document.Controls ?? new List<ControlDocument>())
            .Select(c => new Control(
                c.Id,
                c.Date,
                new HemoglobinLevel(c.HemoglobinLevel)
            )).ToList();

        // Convertir antecedentes
        var antecedentes = (document.Antecedentes ?? new List<AntecedenteDocument>())
            .Select(a => new Antecedente(a.Type, a.Description)).ToList();

        // Convertir síntomas
        var sintomas = document.Sintomas ?? new List<string>();

        return new MedicalRecord(
            document.MedicalRecordId,                          // id
            document.CreatedAt,                                // createdAt
            new Weight(document.Weight),                       // weight
            new Height(document.Height),                       // height
            GenderExtensions.FromString(document.Gender),      // gender
            new MotivoConsulta(document.MotivoConsulta),       // motivoConsulta
            new Observaciones(document.Observaciones ?? string.Empty), // observaciones
            document.PatientId,                                // patientId
            document.NurseId,                                  // nurseId (opcional)
            document.HemoglobinLevel.HasValue 
                ? new HemoglobinLevel(document.HemoglobinLevel.Value) 
                : null,                                        // hemoglobinLevel (opcional)
            antecedentes,                                      // antecedentes
            sintomas,                                          // sintomas
            controls                                           // controls
        );
    }

    public static object ToPersistence(MedicalRecord medicalRecord)
    {
        var data = medicalRecord.ToPrimitives();

        return new
        {
            id = data.Id,
            patientId = data.PatientId,
            nurseId = data.NurseId,
            createdAt = data.CreatedAt,
            updatedAt = data.UpdatedAt,
            hemoglobinLevel = data.HemoglobinLevel,
            weight = data.Weight,
            height = data.Height,
            gender = data.Gender,
            antecedentes = data.Antecedentes,
            motivoConsulta = data.MotivoConsulta,
            observaciones = data.Observaciones,
            sintomas = data.Sintomas,
            controls = data.Controls
        };
    }
}