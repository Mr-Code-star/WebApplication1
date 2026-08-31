using WebApplication1.patient_management.Domain.Aggregate;
using WebApplication1.patient_management.Domain.Enums;
using WebApplication1.patient_management.Domain.ValueObjects;

namespace WebApplication1.patient_management.Infrastructure.Mapper;




public static class PatientMapper
{
    public static Patient ToDomain(dynamic document)
    {
        return new Patient(
            document.id,
            document.name,
            document.lastName,
            new BirthDate((DateTime)document.birthDate),
            new Weight((double)document.currentWeight),
            new Height((double)document.currentHeight),
            document.motherId,
            GenderExtensions.FromString(document.gender),
            document.nurseId,
            document.facilityId,
            PatientStatusExtensions.FromString(document.status)
        );
    }

    public static object ToPersistence(Patient patient)
    {
        var data = patient.ToPrimitives();

        return new
        {
            id = data.Id,
            name = data.Name,
            lastName = data.LastName,
            birthDate = data.BirthDate,
            currentWeight = data.CurrentWeight,
            currentHeight = data.CurrentHeight,
            motherId = data.MotherId,
            nurseId = data.NurseId,
            gender = data.Gender,
            facilityId = data.FacilityId,
            status = data.Status
        };
    }
}