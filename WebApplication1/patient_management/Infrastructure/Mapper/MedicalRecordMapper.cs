using WebApplication1.patient_management.Domain.Entities;
using WebApplication1.patient_management.Domain.Enums;
using WebApplication1.patient_management.Domain.ValueObjects;

namespace WebApplication1.patient_management.Infrastructure.Mapper;

public static class MedicalRecordMapper
{
    public static MedicalRecord ToDomain(dynamic document)
    {
        var controls = ((IEnumerable<dynamic>)document.controls ?? Enumerable.Empty<dynamic>())
            .Select((dynamic control) =>
            {
                return new Control(
                    control.id ?? control._id,
                    control.date ?? control.createdAt,
                    new HemoglobinLevel((double?)control.hemoglobinLevel)
                );
            }).ToList();

        return new MedicalRecord(
            document.id,                                    // 1. id
            document.createdAt,                             // 2. createdAt
            new Weight((double)document.weight),            // 3. weight
            new Height((double)document.height),            // 4. height
            GenderExtensions.FromString(document.gender),   // 5. gender
            new MotivoConsulta(document.motivoConsulta),    // 6. motivoConsulta
            new Observaciones(document.observaciones),      // 7. observaciones
            document.patientId,                             // 8. patientId
            document.nurseId,                               // 9. nurseId (opcional)
            document.hemoglobinLevel != null ? new HemoglobinLevel((double)document.hemoglobinLevel) : null, // 10. hemoglobinLevel (opcional)
            ((IEnumerable<dynamic>)document.antecedentes ?? Enumerable.Empty<dynamic>())
                .Select(a => new Antecedente(a.type, a.description)).ToList(), // 11. antecedentes (opcional)
            ((IEnumerable<dynamic>)document.sintomas ?? Enumerable.Empty<dynamic>()).Select(s => (string)s).ToList(), // 12. sintomas (opcional)
            controls                                        // 13. controls (opcional)
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