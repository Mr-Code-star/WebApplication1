using WebApplication1.HealthyFacility.Domain.Models.Entities;

namespace WebApplication1.HealthyFacility.Infrastructure.Mappers;

public static class NurseAssignmentMapper
{
    public static NurseAssignment ToDomain(dynamic document)
    {
        return new NurseAssignment(
            document.id,
            document.facilityId,
            document.nurseId
        );
    }

    public static object ToPersistence(NurseAssignment assignment)
    {
        var data = assignment.ToPrimitives();

        return new
        {
            id = data.Id,
            facilityId = data.FacilityId,
            nurseId = data.NurseId
        };
    }
}