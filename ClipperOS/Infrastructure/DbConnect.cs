using System;
using System.Threading.Tasks;
using Npgsql;

namespace ClipperOS.Infrastructure
{
    public class DbConnect
    {
        private readonly string _connectionString;
        
        public DbConnect(string connectionString)
        {
            _connectionString = connectionString;
        }
        
        public NpgsqlConnection GetConnection()
        {
            var connection = new NpgsqlConnection(_connectionString);
            return connection;
        }
        public async Task<int> ExecuteNonQueryAsync(string sql, params NpgsqlParameter[] parameters)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(sql, connection);
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            return await command.ExecuteNonQueryAsync();
        }
        
        public async Task<List<T>> ExecuteReaderAsync<T>(string sql, Func<NpgsqlDataReader, T> map, params NpgsqlParameter[] parameters)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(sql, connection);
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            using var reader = await command.ExecuteReaderAsync();
            var results = new List<T>();

            while (await reader.ReadAsync())
            {
                results.Add(map(reader));
            }

            return results;
        }
    }
}