namespace WebApplication1.patient_management.Infrastructure.Persitencia.MongoDb.Models;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


public class PatientDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string PatientId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public double CurrentWeight { get; set; }
    public double CurrentHeight { get; set; }
    public string MotherId { get; set; } = string.Empty;
    public string? NurseId { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? FacilityId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}