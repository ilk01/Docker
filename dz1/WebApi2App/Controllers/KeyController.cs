namespace WebApi2App.Controllers;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using WebApi2App.Models;

[ApiController]
[Route("api/[controller]")]
public class ProfilesController : ControllerBase
{
    private readonly IConfiguration configuration;

    public ProfilesController(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProfileDto>>> GetAll()
    {
        var connectionString = this.configuration.GetConnectionString("Default");
        ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString);

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var cmd = new NpgsqlCommand("SELECT id, full_name, email FROM profiles ORDER BY id;", connection);
        await using var reader = await cmd.ExecuteReaderAsync();

        var results = new List<ProfileDto>();
        while (await reader.ReadAsync())
        {
            results.Add(new ProfileDto
            {
                Id = reader.GetInt32(0),
                FullName = reader.GetString(1),
                Email = reader.GetString(2),
            });
        }

        return this.Ok(results);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProfileDto>> GetById(int id)
    {
        var connectionString = this.configuration.GetConnectionString("Default");
        ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString);

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var cmd = new NpgsqlCommand("SELECT id, full_name, email FROM profiles WHERE id = @id;", connection);
        cmd.Parameters.AddWithValue("id", id);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return this.NotFound();
        }

        return this.Ok(new ProfileDto
        {
            Id = reader.GetInt32(0),
            FullName = reader.GetString(1),
            Email = reader.GetString(2),
        });
    }

    [HttpPost]
    public async Task<ActionResult<ProfileDto>> Create([FromBody] ProfileDto input)
    {
        var connectionString = this.configuration.GetConnectionString("Default");
        ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString);

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "INSERT INTO profiles(full_name, email) VALUES (@full_name, @email) RETURNING id;",
            connection);

        cmd.Parameters.AddWithValue("full_name", input.FullName ?? string.Empty);
        cmd.Parameters.AddWithValue("email", input.Email ?? string.Empty);

        var newId = (int)(await cmd.ExecuteScalarAsync() ?? 0);
        input.Id = newId;

        return this.Created($"/api/profiles/{newId}", input);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProfileDto>> Update(int id, [FromBody] ProfileDto input)
    {
        var connectionString = this.configuration.GetConnectionString("Default");
        ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString);

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "UPDATE profiles SET full_name = @full_name, email = @email WHERE id = @id;",
            connection);

        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("full_name", input.FullName ?? string.Empty);
        cmd.Parameters.AddWithValue("email", input.Email ?? string.Empty);

        var affected = await cmd.ExecuteNonQueryAsync();
        if (affected == 0)
        {
            return this.NotFound();
        }

        input.Id = id;
        return this.Ok(input);
    }
}