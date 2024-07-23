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
        private readonly DatabaseType _databaseType;

        public AdoNetAdapter(string connectionString, DatabaseType databaseType)
        {
            _databaseType = databaseType;
            _connection = CreateDbConnection(connectionString);
        }

        #region private methods

        private DbConnection CreateDbConnection(string connectionString)
        {
            return _databaseType switch
            {
                DatabaseType.SqlServer => new SqlConnection(connectionString),
                DatabaseType.MySql => new MySqlConnection(connectionString),
                DatabaseType.PostgreSql => new NpgsqlConnection(connectionString),
                DatabaseType.SQLite => new SqliteConnection(connectionString),
                DatabaseType.Oracle => new OracleConnection(connectionString),
                _ => throw new NotSupportedException($"Database type {_databaseType} is not supported.")
            };
        }

        private void AddParameters<Tin>(DbCommand command, Tin parameters)
        {
            if (parameters == null) return;

            var properties = parameters.GetType().GetProperties();
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

        #endregion

        #region public methods

        public async Task<IEnumerable<Tout>> FindAsync<Tout, Tin>(string query,
                                                                  Tin parameters,
                                                                  CommandType commandType,
                                                                  CancellationToken cancellationToken = default)
        {
            var result = new List<Tout>();

            using var command = _connection.CreateCommand();
            command.CommandText = query;
            command.CommandType = commandType;
            command.Transaction = _transaction;
            AddParameters(command, parameters);

            if (_transaction is null)
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

        public async Task<Tout?> FindOneAsync<Tout, Tin>(string query,
                                                         Tin parameters,
                                                         CommandType commandType,
                                                         CancellationToken cancellationToken = default)
        {
            using var command = _connection.CreateCommand();
            command.CommandText = query;
            command.CommandType = commandType;
            command.Transaction = _transaction;
            AddParameters(command, parameters);

            if (_transaction is null)
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

        public async Task<IEnumerable<Tout>> SaveAndFindAsync<Tout, Tin>(string query,
                                                                 Tin parameters,
                                                                 CommandType commandType,
                                                                 CancellationToken cancellationToken = default)
        {
            var result = new List<Tout>();

            using var command = _connection.CreateCommand();
            command.CommandText = query;
            command.CommandType = commandType;
            command.Transaction = _transaction;
            AddParameters(command, parameters);

            if (_transaction is null)
            {
                await _connection.OpenAsync(cancellationToken);
                await command.ExecuteNonQueryAsync(cancellationToken);

                using var reader = await command.ExecuteReaderAsync(cancellationToken);
                while (await reader.ReadAsync(cancellationToken))
                {
                    result.Add(MapReaderToEntity<Tout>(reader));
                }
                await _connection.CloseAsync();
            }
            else
            {
                await command.ExecuteNonQueryAsync(cancellationToken);

                using var reader = await command.ExecuteReaderAsync(cancellationToken);
                while (await reader.ReadAsync(cancellationToken))
                {
                    result.Add(MapReaderToEntity<Tout>(reader));
                }
            }

            return result;
        }


        public async Task<Tout?> SaveAndFindOneAsync<Tout, Tin>(string query,
                                                                Tin parameters,
                                                                CommandType commandType,
                                                                CancellationToken cancellationToken = default)
        {
            using var command = _connection.CreateCommand();
            command.CommandText = query;
            command.CommandType = commandType;
            command.Transaction = _transaction;
            AddParameters(command, parameters);

            if (_transaction is null)
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

        public async Task SaveAsync<Tin>(string query,
                                         Tin parameters,
                                         CommandType commandType,
                                         CancellationToken cancellationToken = default)
        {
            using var command = _connection.CreateCommand();
            command.CommandText = query;
            command.CommandType = commandType;
            command.Transaction = _transaction;
            AddParameters<Tin>(command, parameters);

            if (_transaction is null)
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

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_connection.State == ConnectionState.Closed)
            {
                await _connection.OpenAsync(cancellationToken);
            }

            _transaction = await _connection.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction is not null)
            {
                await _transaction.CommitAsync(cancellationToken);
                await _connection.CloseAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction is not null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                await _connection.CloseAsync();
                _transaction = null;
            }
        }

        #endregion
    }
}