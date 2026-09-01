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
        try
        {
            _logger.LogInformation("📝 Guardando posta: {FacilityName}", facility.Name);

            // ✅ Usar el DTO de persistencia
            var data = HealthFacilityMapper.ToPersistence(facility);

            _logger.LogInformation("📄 Datos mapeados: {@Data}", new
            {
                data.Id,
                data.Name,
                data.Address,
                data.DistrictId,
                data.DistrictName,
                data.Latitude,
                data.Longitude,
                data.PhoneNumber,
                ServicesCount = data.Services.Count,
                AvailableDaysCount = data.AvailableDays.Count,
                AvailableSlotsCount = data.AvailableSlots.Count,
                data.ScheduleOfOperation,
                data.Status
            });

            var document = new HealthFacilityDocument
            {
                HealthFacilityId = data.Id,
                Name = data.Name,
                Address = data.Address,
                DistrictId = data.DistrictId,
                DistrictName = data.DistrictName,
                Coordinates = new CoordinatesDocument
                {
                    Lat = data.Latitude,
                    Lng = data.Longitude
                },
                PhoneNumber = data.PhoneNumber,
                Services = data.Services ?? new List<string>(),
                OperatingSchedule = new OperatingScheduleDocument
                {
                    AvailableDays = data.AvailableDays ?? new List<string>(),
                    AvailableSlots = data.AvailableSlots ?? new List<string>()
                },
                ScheduleOfOperation = data.ScheduleOfOperation,
                Status = data.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _collection.InsertOneAsync(document);
            _logger.LogInformation("✅ Posta de salud creada: {HealthFacilityId}", document.HealthFacilityId);

            return facility;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al guardar posta: {FacilityName}", facility.Name);
            throw;
        }
    }

    public async Task<HealthFacility?> FindByIdAsync(string id)
    {
        var filter = Builders<HealthFacilityDocument>.Filter.Eq(x => x.HealthFacilityId, id);
        var document = await _collection.Find(filter).FirstOrDefaultAsync();

        if (document == null) return null;

        // ✅ Convertir documento a objeto dinámico para el mapper
        var dynamicDoc = new
        {
            id = document.HealthFacilityId,
            name = document.Name,
            address = document.Address,
            districtId = document.DistrictId,
            districtName = document.DistrictName,
            coordinates = new { lat = document.Coordinates.Lat, lng = document.Coordinates.Lng },
            phoneNumber = document.PhoneNumber,
            services = document.Services,
            operatingSchedule = new
            {
                availableDays = document.OperatingSchedule.AvailableDays,
                availableSlots = document.OperatingSchedule.AvailableSlots
            },
            scheduleOfOperation = document.ScheduleOfOperation,
            status = document.Status,
            nurseAssignments = new List<object>() // Asignaciones vacías por ahora
        };

        return HealthFacilityMapper.ToDomain(dynamicDoc);
    }

    public async Task<List<HealthFacility>> FindAllAsync()
    {
        var documents = await _collection.Find(_ => true).ToListAsync();
        var result = new List<HealthFacility>();

        foreach (var doc in documents)
        {
            var dynamicDoc = new
            {
                id = doc.HealthFacilityId,
                name = doc.Name,
                address = doc.Address,
                districtId = doc.DistrictId,
                districtName = doc.DistrictName,
                coordinates = new { lat = doc.Coordinates.Lat, lng = doc.Coordinates.Lng },
                phoneNumber = doc.PhoneNumber,
                services = doc.Services,
                operatingSchedule = new
                {
                    availableDays = doc.OperatingSchedule.AvailableDays,
                    availableSlots = doc.OperatingSchedule.AvailableSlots
                },
                scheduleOfOperation = doc.ScheduleOfOperation,
                status = doc.Status,
                nurseAssignments = new List<object>()
            };

            result.Add(HealthFacilityMapper.ToDomain(dynamicDoc));
        }

        return result;
    }

    public async Task<List<HealthFacility>> FindActiveFacilitiesAsync()
    {
        var filter = Builders<HealthFacilityDocument>.Filter.Eq(x => x.Status, FacilityStatus.ACTIVE.ToStringValue());
        var documents = await _collection.Find(filter).ToListAsync();

        var result = new List<HealthFacility>();

        foreach (var doc in documents)
        {
            var dynamicDoc = new
            {
                id = doc.HealthFacilityId,
                name = doc.Name,
                address = doc.Address,
                districtId = doc.DistrictId,
                districtName = doc.DistrictName,
                coordinates = new { lat = doc.Coordinates.Lat, lng = doc.Coordinates.Lng },
                phoneNumber = doc.PhoneNumber,
                services = doc.Services,
                operatingSchedule = new
                {
                    availableDays = doc.OperatingSchedule.AvailableDays,
                    availableSlots = doc.OperatingSchedule.AvailableSlots
                },
                scheduleOfOperation = doc.ScheduleOfOperation,
                status = doc.Status,
                nurseAssignments = new List<object>()
            };

            result.Add(HealthFacilityMapper.ToDomain(dynamicDoc));
        }

        return result;
    }

    public async Task UpdateAsync(HealthFacility facility)
    {
        var data = HealthFacilityMapper.ToPersistence(facility);
        var filter = Builders<HealthFacilityDocument>.Filter.Eq(x => x.HealthFacilityId, data.Id);

        var update = Builders<HealthFacilityDocument>.Update
            .Set(x => x.Name, data.Name)
            .Set(x => x.Address, data.Address)
            .Set(x => x.DistrictId, data.DistrictId)
            .Set(x => x.DistrictName, data.DistrictName)
            .Set(x => x.Coordinates, new CoordinatesDocument { Lat = data.Latitude, Lng = data.Longitude })
            .Set(x => x.PhoneNumber, data.PhoneNumber)
            .Set(x => x.Services, data.Services ?? new List<string>())
            .Set(x => x.OperatingSchedule, new OperatingScheduleDocument
            {
                AvailableDays = data.AvailableDays ?? new List<string>(),
                AvailableSlots = data.AvailableSlots ?? new List<string>()
            })
            .Set(x => x.ScheduleOfOperation, data.ScheduleOfOperation)
            .Set(x => x.Status, data.Status)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        await _collection.UpdateOneAsync(filter, update);
        _logger.LogInformation("Posta de salud actualizada: {HealthFacilityId}", data.Id);
    }
}

