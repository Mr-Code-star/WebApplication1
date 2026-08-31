using WebApplication1.NutritionDiary.Domain.Models.Entities;
using WebApplication1.NutritionDiary.Domain.Repositories;
using WebApplication1.NutritionDiary.Infrastructure.Mappers;
using WebApplication1.NutritionDiary.Infrastructure.Persitencia.Models;

namespace WebApplication1.NutritionDiary.Infrastructure.Persitencia.Repositories;

using Microsoft.Extensions.Logging;
using MongoDB.Driver;


public class MongoFoodEntryRepository : IFoodEntryRepository
{
    private readonly IMongoCollection<FoodEntryDocument> _collection;
    private readonly ILogger<MongoFoodEntryRepository> _logger;

    public MongoFoodEntryRepository(IMongoDatabase database, ILogger<MongoFoodEntryRepository> logger)
    {
        _collection = database.GetCollection<FoodEntryDocument>("foodentries");
        _logger = logger;
    }

    public async Task SaveAsync(FoodEntry entry)
    {
        var data = FoodEntryMapper.ToPersistence(entry);

        var document = new FoodEntryDocument
        {
            FoodEntryId = (string)data.GetType().GetProperty("id")?.GetValue(data, null)!,
            DiaryId = (string)data.GetType().GetProperty("diaryId")?.GetValue(data, null)!,
            FoodItemId = (string)data.GetType().GetProperty("foodItemId")?.GetValue(data, null)!,
            Quantity = (double)data.GetType().GetProperty("quantity")?.GetValue(data, null)!,
            Unit = (string)data.GetType().GetProperty("unit")?.GetValue(data, null)!,
            IronContributed = (double)data.GetType().GetProperty("ironContributed")?.GetValue(data, null)!,
            RegisteredAt = (DateTime)data.GetType().GetProperty("registeredAt")?.GetValue(data, null)!
        };

        await _collection.InsertOneAsync(document);
        _logger.LogInformation("Food entry guardado: {FoodEntryId}", document.FoodEntryId);
    }

    public async Task<List<FoodEntry>> FindByDiaryIdAsync(string diaryId)
    {
        var filter = Builders<FoodEntryDocument>.Filter.Eq(x => x.DiaryId, diaryId);
        var documents = await _collection.Find(filter).ToListAsync();

        return documents.Select(FoodEntryMapper.ToDomain).ToList();
    }

    public async Task<int> CountByDiaryIdAsync(string diaryId)
    {
        var filter = Builders<FoodEntryDocument>.Filter.Eq(x => x.DiaryId, diaryId);
        return (int)await _collection.CountDocumentsAsync(filter);
    }
}