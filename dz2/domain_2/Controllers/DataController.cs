using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using domain_2.Services;
using System.Text.Json;

namespace domain_2.Controllers;

[ApiController]
[Route("[controller]")]
public class DataController : ControllerBase
{
    private readonly IDatabase _redisDb;
    private readonly DataService _dataService;

    public DataController(IConnectionMultiplexer redis, DataService dataService)
    {
        _redisDb = redis.GetDatabase();
        _dataService = dataService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetData(string id)
    {
        string cacheKey = $"data:{id}";

        var cachedData = await _redisDb.StringGetAsync(cacheKey);

        if (!cachedData.IsNullOrEmpty)
        {
            return Ok(new { source = "cache", data = JsonSerializer.Deserialize<string>(cachedData) });
        }

        var data = await _dataService.GetDataFromDatabase(id);

        await _redisDb.StringSetAsync(cacheKey, JsonSerializer.Serialize(data), TimeSpan.FromMinutes(1));

        return Ok(new { source = "database", data = data });
    }
}
