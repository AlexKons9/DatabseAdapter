using DatabaseAdapter.Domain.Enums;
using DatabaseAdapter.Domain.Interfaces.SqlAdapters;
using DatabaseAdapter.Infrastructure.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySqlConnector;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.Common;

namespace DatabaseAdapter.DataHandlers.SqlAdapters
{
    public class AdoNetAdapter : ISqlAdapter
    {
        private readonly DbConnection _connection;
        private DbTransaction? _transaction;
        private readonly SqlDatabaseType _databaseType;

        public AdoNetAdapter(string connectionString, SqlDatabaseType databaseType)
        {
            _databaseType = databaseType;
            _connection = CreateDbConnection(connectionString);
        }

        private DbConnection CreateDbConnection(string connectionString)
        {
            return _databaseType switch
            {
                SqlDatabaseType.SqlServer => new SqlConnection(connectionString),
                SqlDatabaseType.MySql => new MySqlConnection(connectionString),
                SqlDatabaseType.PostgreSql => new NpgsqlConnection(connectionString),
                SqlDatabaseType.SQLite => new SqliteConnection(connectionString),
                SqlDatabaseType.Oracle => new OracleConnection(connectionString),
                _ => throw new NotSupportedException($"Database type {_databaseType} is not supported.")
            };
        }

        public async Task SaveAsync<Tin>(string query, Tin parameters, CommandType commandType, CancellationToken cancellationToken)
        {
            using var command = _connection.CreateCommand();
            command.CommandText = query;
            command.CommandType = commandType;
            command.Transaction = _transaction;
            AddParameters(command, parameters);

            if (_transaction == null)
            {
                await _connection.OpenAsync();
                await command.ExecuteNonQueryAsync(cancellationToken);
                await _connection.CloseAsync();
            }
            else
            {
                await command.ExecuteNonQueryAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<Tout>> FindAsync<Tout, Tin>(string query, Tin parameters, CommandType commandType, CancellationToken cancellationToken)
        {
            var result = new List<Tout>();

            using var command = _connection.CreateCommand();
            command.CommandText = query;
            command.CommandType = commandType;
            command.Transaction = _transaction;
            AddParameters(command, parameters);

            if (_transaction == null)
            {
                await _connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync(cancellationToken);
                while (await reader.ReadAsync())
                {
                    result.Add(MapReaderToEntity<Tout>(reader));
                }
                await _connection.CloseAsync();
            }
            else
            {
                using var reader = await command.ExecuteReaderAsync(cancellationToken);
                while (await reader.ReadAsync())
                {
                    result.Add(MapReaderToEntity<Tout>(reader));
                }
            }

            return result;
        }

        public async Task<Tout?> FindOneAsync<Tout, Tin>(string query, Tin parameters, CommandType commandType, CancellationToken cancellationToken)
        {
            using var command = _connection.CreateCommand();
            command.CommandText = query;
            command.CommandType = commandType;
            command.Transaction = _transaction;
            AddParameters(command, parameters);

            if (_transaction == null)
            {
                await _connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync(cancellationToken);
                Tout? result = default;
                if (await reader.ReadAsync(cancellationToken))
                {
                    result = MapReaderToEntity<Tout>(reader);
                }
                await _connection.CloseAsync();
                return result;
            }
            else
            {
                using var reader = await command.ExecuteReaderAsync(cancellationToken);
                Tout? result = default;
                if (await reader.ReadAsync(cancellationToken))
                {
                    result = MapReaderToEntity<Tout>(reader);
                }
                return result;
            }
        }

        public async Task<Tout?> FindOneAndSaveAsync<Tout, Tin>(string query, Tin parameters, CommandType commandType, CancellationToken cancellationToken)
        {
            using var command = _connection.CreateCommand();
            command.CommandText = query;
            command.CommandType = commandType;
            command.Transaction = _transaction;
            AddParameters(command, parameters);

            if (_transaction == null)
            {
                await _connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync(cancellationToken);
                Tout? result = default;
                if (await reader.ReadAsync(cancellationToken))
                {
                    result = MapReaderToEntity<Tout>(reader);
                }
                await command.ExecuteNonQueryAsync(cancellationToken);
                await _connection.CloseAsync();
                return result;
            }
            else
            {
                using var reader = await command.ExecuteReaderAsync(cancellationToken);
                Tout? result = default;
                if (await reader.ReadAsync(cancellationToken))
                {
                    result = MapReaderToEntity<Tout>(reader);
                }
                await command.ExecuteNonQueryAsync(cancellationToken);
                return result;
            }
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            if (_connection.State == ConnectionState.Closed)
            {
                await _connection.OpenAsync(cancellationToken);
            }

            _transaction = await _connection.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
                await _connection.CloseAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                await _connection.CloseAsync();
                _transaction = null;
            }
        }

        private void AddParameters<T>(DbCommand command, T parameters)
        {
            if (parameters == null) return;

            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = $"@{property.Name}";
                parameter.Value = property.GetValue(parameters) ?? DBNull.Value;
                command.Parameters.Add(parameter);
            }
        }

        private Tout MapReaderToEntity<Tout>(DbDataReader reader)
        {
            var entity = Activator.CreateInstance<Tout>();
            var properties = typeof(Tout).GetProperties();

            foreach (var property in properties)
            {
                if (reader.HasColumn(property.Name))
                {
                    var value = reader[property.Name];
                    if (value != DBNull.Value)
                    {
                        property.SetValue(entity, value);
                    }
                }
            }

            return entity;
        }
    }
}
