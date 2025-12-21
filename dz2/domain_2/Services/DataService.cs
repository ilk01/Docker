namespace domain_2.Services;

public class DataService
{
    private readonly string _connectionString;

    public DataService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<string> GetDataFromDatabase(string id)
    {
        await using var conn = new Npgsql.NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        await using (var cmd = new Npgsql.NpgsqlCommand(
                         "CREATE TABLE IF NOT EXISTS domain2_data (id text PRIMARY KEY, value text NOT NULL);",
                         conn))
        {
            await cmd.ExecuteNonQueryAsync();
        }

        await using (var cmd = new Npgsql.NpgsqlCommand(
                         "SELECT value FROM domain2_data WHERE id = @id;",
                         conn))
        {
            cmd.Parameters.AddWithValue("id", id);
            var result = await cmd.ExecuteScalarAsync();
            if (result is string existing)
            {
                return existing;
            }
        }

        var generated = $"This is the data for {id} from Postgres.";

        await using (var cmd = new Npgsql.NpgsqlCommand(
                         "INSERT INTO domain2_data (id, value) VALUES (@id, @value);",
                         conn))
        {
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("value", generated);
            await cmd.ExecuteNonQueryAsync();
        }

        return generated;
    }
}
