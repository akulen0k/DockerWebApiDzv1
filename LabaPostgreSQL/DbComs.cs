using Dapper;
using Npgsql;
using NpgsqlTypes;

namespace LabaPostgreSQL;
// Open и OpenAsync??? спросить про асинхронность
// await using??
public static class DbComs
{
    public static async Task WriteToDb(string connectionString, string dbName, QueryInfo qi)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            var command = new NpgsqlCommand(@$"INSERT INTO {dbName} (handle, data) VALUES (@han, @dat)", connection);
            command.Parameters.AddWithValue("han", qi.handle);
            command.Parameters.AddWithValue("dat", NpgsqlDbType.Jsonb, qi.data);
            
            await command.ExecuteNonQueryAsync(); // okey?
        }
    }

    public static async Task<QueryInfo[]> GetAllQueries(string connectionString, string dbName)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            var command = @$"SELECT * FROM {dbName}";
            var listOfQueries = await connection.QueryAsync<QueryInfo>(command);
            
            return listOfQueries.ToArray();
        }
    }

    public static void CreateDatabase(string connectionString, string dbName)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            var command = @$"CREATE TABLE IF NOT EXISTS {dbName}(
                        id SERIAL PRIMARY KEY,
                        handle TEXT NOT NULL,
                        data JSONB NOT NULL
                        )";
            connection.Execute(command);
        }
    }
}