using WebApplication1.TreatmentTracking.Domain.Model.Aggregate;
using WebApplication1.TreatmentTracking.Domain.Model.Entities;
using WebApplication1.TreatmentTracking.Domain.Model.ValueObjects;

namespace WebApplication1.TreatmentTracking.Infrastructure.Mappers;

public static class TreatmentMapper
{
    public static Treatment ToDomain(dynamic document)
    {
        var riskScore = new RiskScore(
            document.riskScore.id,
            document.riskScore.score,
            RiskLevelExtensions.FromString(document.riskScore.riskLevel ?? "LOW"),
            document.riskScore.calculatedAt ?? DateTime.UtcNow
        );

        return new Treatment(
            document.id,
            document.patientId,
            document.nurseId,
            document.supplement,
            document.quantity,
            document.dosingHours,
            document.durationDays,
            document.startDate,
            document.endDate,
            TreatmentStatusExtensions.FromString(document.status),
            document.adherenceScore,
            document.currentStreak,
            document.totalConfirmed,
            document.totalOmitted,
            document.completionObservation,
            document.abandonmentObservation,
            riskScore
        );
    }

    public static object ToPersistence(Treatment treatment)
    {
        var data = treatment.ToPrimitives();

        return new
        {
            id = data.Id,
            patientId = data.PatientId,
            nurseId = data.NurseId,
            supplement = data.Supplement,
            quantity = data.Quantity,
            dosingHours = data.DosingHours,
            durationDays = data.DurationDays,
            startDate = data.StartDate,
            endDate = data.EndDate,
            status = data.Status,
            adherenceScore = data.AdherenceScore,
            currentStreak = data.CurrentStreak,
            totalConfirmed = data.TotalConfirmed,
            totalOmitted = data.TotalOmitted,
            completionObservation = data.CompletionObservation,
            abandonmentObservation = data.AbandonmentObservation,
            riskScore = new
            {
                id = data.RiskScore.Id,
                score = data.RiskScore.Score,
                riskLevel = data.RiskScore.RiskLevel,
                calculatedAt = data.RiskScore.CalculatedAt
            }
        };
    }
}