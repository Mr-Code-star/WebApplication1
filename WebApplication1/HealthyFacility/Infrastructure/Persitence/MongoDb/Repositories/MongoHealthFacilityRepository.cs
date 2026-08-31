using MongoDB.Driver;
using WebApplication1.HealthyFacility.Domain.Models.Aggregate;
using WebApplication1.HealthyFacility.Domain.Models.ValueObjects;
using WebApplication1.HealthyFacility.Domain.Repositories;
using WebApplication1.HealthyFacility.Infrastructure.Mappers;
using WebApplication1.HealthyFacility.Infrastructure.Persitence.MongoDb.Models;

namespace WebApplication1.HealthyFacility.Infrastructure.Persitence.MongoDb.Repositories;

public class MongoHealthFacilityRepository : IHealthFacilityRepository
{
    private readonly IMongoCollection<HealthFacilityDocument> _collection;
    private readonly ILogger<MongoHealthFacilityRepository> _logger;

    public MongoHealthFacilityRepository(IMongoDatabase database, ILogger<MongoHealthFacilityRepository> logger)
    {
        _collection = database.GetCollection<HealthFacilityDocument>("healthfacilities");
        _logger = logger;
    }

    public async Task<HealthFacility> SaveAsync(HealthFacility facility)
    {
        var data = HealthFacilityMapper.ToPersistence(facility);

        var document = new HealthFacilityDocument
        {
            HealthFacilityId = (string)data.GetType().GetProperty("id")?.GetValue(data, null)!,
            Name = (string)data.GetType().GetProperty("name")?.GetValue(data, null)!,
            Address = (string)data.GetType().GetProperty("address")?.GetValue(data, null)!,
            DistrictId = (string)data.GetType().GetProperty("districtId")?.GetValue(data, null)!,
            DistrictName = (string)data.GetType().GetProperty("districtName")?.GetValue(data, null)!,
            Coordinates = new CoordinatesDocument
            {
                Lat = (double)data.GetType().GetProperty("coordinates")?.GetType().GetProperty("lat")?.GetValue(data.GetType().GetProperty("coordinates")?.GetValue(data, null), null)!,
                Lng = (double)data.GetType().GetProperty("coordinates")?.GetType().GetProperty("lng")?.GetValue(data.GetType().GetProperty("coordinates")?.GetValue(data, null), null)!
            },
            PhoneNumber = (string)data.GetType().GetProperty("phoneNumber")?.GetValue(data, null)!,
            Services = ((IEnumerable<dynamic>)data.GetType().GetProperty("services")?.GetValue(data, null) ?? Enumerable.Empty<dynamic>())
                .Select(s => (string)s).ToList(),
            OperatingSchedule = new OperatingScheduleDocument
            {
                AvailableDays = ((IEnumerable<dynamic>)data.GetType().GetProperty("operatingSchedule")?.GetType().GetProperty("availableDays")?.GetValue(data.GetType().GetProperty("operatingSchedule")?.GetValue(data, null), null) ?? Enumerable.Empty<dynamic>())
                    .Select(d => (string)d).ToList(),
                AvailableSlots = ((IEnumerable<dynamic>)data.GetType().GetProperty("operatingSchedule")?.GetType().GetProperty("availableSlots")?.GetValue(data.GetType().GetProperty("operatingSchedule")?.GetValue(data, null), null) ?? Enumerable.Empty<dynamic>())
                    .Select(s => (string)s).ToList()
            },
            ScheduleOfOperation = (string)data.GetType().GetProperty("scheduleOfOperation")?.GetValue(data, null)!,
            Status = (string)data.GetType().GetProperty("status")?.GetValue(data, null)!,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _collection.InsertOneAsync(document);
        _logger.LogInformation("Posta de salud creada: {HealthFacilityId}", document.HealthFacilityId);

        return facility;
    }

    public async Task<HealthFacility?> FindByIdAsync(string id)
    {
        var filter = Builders<HealthFacilityDocument>.Filter.Eq(x => x.HealthFacilityId, id);
        var document = await _collection.Find(filter).FirstOrDefaultAsync();

        if (document == null) return null;

        return HealthFacilityMapper.ToDomain(document);
    }

    public async Task<List<HealthFacility>> FindAllAsync()
    {
        var documents = await _collection.Find(_ => true).ToListAsync();
        return documents.Select(HealthFacilityMapper.ToDomain).ToList();
    }

    public async Task<List<HealthFacility>> FindActiveFacilitiesAsync()
    {
        var filter = Builders<HealthFacilityDocument>.Filter.Eq(x => x.Status, FacilityStatus.ACTIVE.ToStringValue());
        var documents = await _collection.Find(filter).ToListAsync();

        return documents.Select(HealthFacilityMapper.ToDomain).ToList();
    }

    public async Task UpdateAsync(HealthFacility facility)
    {
        var data = HealthFacilityMapper.ToPersistence(facility);
        var facilityId = (string)data.GetType().GetProperty("id")?.GetValue(data, null)!;

        var filter = Builders<HealthFacilityDocument>.Filter.Eq(x => x.HealthFacilityId, facilityId);

        var update = Builders<HealthFacilityDocument>.Update
            .Set(x => x.Name, (string)data.GetType().GetProperty("name")?.GetValue(data, null)!)
            .Set(x => x.Address, (string)data.GetType().GetProperty("address")?.GetValue(data, null)!)
            .Set(x => x.PhoneNumber, (string)data.GetType().GetProperty("phoneNumber")?.GetValue(data, null)!)
            .Set(x => x.Services, ((IEnumerable<dynamic>)data.GetType().GetProperty("services")?.GetValue(data, null) ?? Enumerable.Empty<dynamic>())
                .Select(s => (string)s).ToList())
            .Set(x => x.Status, (string)data.GetType().GetProperty("status")?.GetValue(data, null)!)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        await _collection.UpdateOneAsync(filter, update);
        _logger.LogInformation("Posta de salud actualizada: {HealthFacilityId}", facilityId);
    }
}