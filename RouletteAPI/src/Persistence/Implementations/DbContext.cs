using System;
using System.Text.Json;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace RoulettesAPI.Persistence.Implementations
{
    public class DbContext : IDbContext
    {   
        private readonly string _host;
        private readonly string _username;
        private readonly string _dbName;
        private readonly string _password;
        private readonly string _port;
        private readonly NpgsqlConnection _connection;

        private string _functionName;

        public DbContext()
        {
            _host = Environment.GetEnvironmentVariable("DB_HOST");
            _username = Environment.GetEnvironmentVariable("DB_USERNAME");
            _dbName = Environment.GetEnvironmentVariable("DB_NAME");
            _password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
            _port = Environment.GetEnvironmentVariable("DB_PORT");
            _connection = new NpgsqlConnection(GenerateConnectionString());
        }
        public async Task<T> ExecuteFunction<T>(string inputObjectStringify, string functionName = null)
        {
            var result = string.Empty;
            try
            {
                await _connection.OpenAsync();
                functionName ??= _functionName;
                await using (var command = new NpgsqlCommand($"SELECT {functionName}(@in_data) ", _connection))
                {
                    command.Parameters.AddWithValue("in_data", NpgsqlDbType.Text, inputObjectStringify);
                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        result = reader.GetString(0);
                        Console.WriteLine(result);
                    }
                }
                return JsonSerializer.Deserialize<T>(result);
            }
            catch
            {
                throw;
            }
            finally
            {
                await _connection.CloseAsync();
                _functionName = string.Empty;
            }
        }

        public IDbContext WithFunctionName(string functionName)
        {
            _functionName = functionName;
            return this;
        }

        private string GenerateConnectionString()
        {
            return $"Host={_host};Port={_port};Username={_username};Password={_password};Database={_dbName}";
        }
    }
}