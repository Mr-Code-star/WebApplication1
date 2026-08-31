using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication1.TreatmentTracking.Infrastructure.Persitencia.MongoDb.Models;

public class RiskScoreDocument
{
    public string Id { get; set; } = string.Empty;
    public int Score { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    public DateTime CalculatedAt { get; set; }
}

public class TreatmentDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string TreatmentId { get; set; } = string.Empty;
    public string PatientId { get; set; } = string.Empty;
    public string NurseId { get; set; } = string.Empty;
    public string Supplement { get; set; } = string.Empty;
    public string Quantity { get; set; } = string.Empty;
    public string DosingHours { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public double AdherenceScore { get; set; }
    public int CurrentStreak { get; set; }
    public int TotalConfirmed { get; set; }
    public int TotalOmitted { get; set; }
    public string? CompletionObservation { get; set; }
    public string? AbandonmentObservation { get; set; }
    public RiskScoreDocument RiskScore { get; set; } = new();
}