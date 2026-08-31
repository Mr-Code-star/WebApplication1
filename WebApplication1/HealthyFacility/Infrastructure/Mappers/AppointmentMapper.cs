using WebApplication1.HealthyFacility.Domain.Models.Entities;
using WebApplication1.HealthyFacility.Domain.Models.ValueObjects;

namespace WebApplication1.HealthyFacility.Infrastructure.Mappers;


public static class AppointmentMapper
{
    public static Appointment ToDomain(dynamic document)
    {
        return new Appointment(
            document.id,
            document.facilityId,
            document.patientId,
            document.motherId,
            document.appointmentDate,
            document.appointmentTime,
            document.nurseId,
            AppointmentStatusExtensions.FromString(document.status)
        );
    }

    public static object ToPersistence(Appointment appointment)
    {
        var data = appointment.ToPrimitives();

        return new
        {
            id = data.Id,
            facilityId = data.FacilityId,
            patientId = data.PatientId,
            motherId = data.MotherId,
            nurseId = data.NurseId,
            appointmentDate = data.AppointmentDate,
            appointmentTime = data.AppointmentTime,
            status = data.Status
        };
    }
}