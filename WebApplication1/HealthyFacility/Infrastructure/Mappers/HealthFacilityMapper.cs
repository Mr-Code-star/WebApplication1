using WebApplication1.HealthyFacility.Domain.Models.Aggregate;
using WebApplication1.HealthyFacility.Domain.Models.Entities;
using WebApplication1.HealthyFacility.Domain.Models.ValueObjects;

namespace WebApplication1.HealthyFacility.Infrastructure.Mappers;

public static class HealthFacilityMapper
{
    public static HealthFacility ToDomain(dynamic document)
    {
        var nurseAssignments = ((IEnumerable<dynamic>)document.nurseAssignments ?? Enumerable.Empty<dynamic>())
            .Select((dynamic assignment) =>
                new NurseAssignment(
                    assignment.id,
                    assignment.facilityId,
                    assignment.nurseId
                )
            ).ToList();

        return new HealthFacility(
            document.id,
            document.name,
            document.address,
            document.districtId,
            document.districtName,
            new Coordinates(
                document.coordinates.lat,
                document.coordinates.lng
            ),
            document.phoneNumber,
            ((IEnumerable<dynamic>)document.services ?? Enumerable.Empty<dynamic>()).Select(s => (string)s).ToList(),
            new OperatingSchedule(
                ((IEnumerable<dynamic>)document.operatingSchedule.availableDays ?? Enumerable.Empty<dynamic>()).Select(d => (string)d).ToList(),
                ((IEnumerable<dynamic>)document.operatingSchedule.availableSlots ?? Enumerable.Empty<dynamic>()).Select(s => (string)s).ToList()
            ),
            document.scheduleOfOperation,
            FacilityStatusExtensions.FromString(document.status),
            nurseAssignments
        );
    }

    public static HealthFacilityPersistenceDto ToPersistence(HealthFacility facility)
    {
        if (facility == null)
            throw new ArgumentNullException(nameof(facility));

        var data = facility.ToPrimitives();

        // ✅ Crear un DTO con valores por defecto para evitar null
        return new HealthFacilityPersistenceDto
        {
            Id = data.Id ?? string.Empty,
            Name = data.Name ?? string.Empty,
            Address = data.Address ?? string.Empty,
            DistrictId = data.DistrictId ?? string.Empty,
            DistrictName = data.DistrictName ?? string.Empty,
            Latitude = data.Coordinates?.Lat ?? 0,
            Longitude = data.Coordinates?.Lng ?? 0,
            PhoneNumber = data.PhoneNumber ?? string.Empty,
            Services = data.Services ?? new List<string>(),
            AvailableDays = data.OperatingSchedule?.AvailableDays ?? new List<string>(),
            AvailableSlots = data.OperatingSchedule?.AvailableSlots ?? new List<string>(),
            ScheduleOfOperation = data.ScheduleOfOperation ?? string.Empty,
            Status = data.Status ?? FacilityStatus.ACTIVE.ToStringValue(),
            NurseAssignments = data.NurseAssignments ?? new List<NurseAssignment.NurseAssignmentPrimitives>()
        };
    }

    public class HealthFacilityPersistenceDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string DistrictId { get; set; } = string.Empty;
        public string DistrictName { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public List<string> Services { get; set; } = new();
        public List<string> AvailableDays { get; set; } = new();
        public List<string> AvailableSlots { get; set; } = new();
        public string ScheduleOfOperation { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<NurseAssignment.NurseAssignmentPrimitives> NurseAssignments { get; set; } = new();
    }
}