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

    public static object ToPersistence(HealthFacility facility)
    {
        var data = facility.ToPrimitives();

        return new
        {
            id = data.Id,
            name = data.Name,
            address = data.Address,
            districtId = data.DistrictId,
            districtName = data.DistrictName,
            coordinates = new
            {
                lat = data.Coordinates.Lat,
                lng = data.Coordinates.Lng
            },
            phoneNumber = data.PhoneNumber,
            services = data.Services,
            operatingSchedule = new
            {
                availableDays = data.OperatingSchedule.AvailableDays,
                availableSlots = data.OperatingSchedule.AvailableSlots
            },
            scheduleOfOperation = data.ScheduleOfOperation,
            status = data.Status,
            nurseAssignments = data.NurseAssignments
        };
    }
}