using DatabaseAdapter.Domain.Enums;
using DatabaseAdapter.Domain.Interfaces.SqlAdapters;
using DatabaseAdapter.Infrastructure.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySqlConnector;
using Npgsql;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;

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
                _ => throw new NotSupportedException($"Database type {_databaseType} is not supported.")
            };
        }

        private static void AddParameters<Tin>(DbCommand command, Tin parameters)
        {
            if (parameters == null) return;

            var properties = parameters.GetType().GetProperties();
            foreach (var property in properties)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = $"@{property.Name}";
                var value = property.GetValue(parameters) ?? DBNull.Value;
                parameter.DbType = GetDbType(property.PropertyType);
                parameter.Value = value;
                command.Parameters.Add(parameter);
            }
        }

        private static DbType GetDbType(Type propertyType)
        {
            switch (Type.GetTypeCode(propertyType))
            {
                case TypeCode.Boolean:
                    return DbType.Boolean;
                case TypeCode.Byte:
                    return DbType.Byte;
                case TypeCode.Char:
                    return DbType.StringFixedLength;
                case TypeCode.DateTime:
                    return DbType.DateTime;
                case TypeCode.Decimal:
                    return DbType.Decimal;
                case TypeCode.Double:
                    return DbType.Double;
                case TypeCode.Int16:
                    return DbType.Int16;
                case TypeCode.Int32:
                    return DbType.Int32;
                case TypeCode.Int64:
                    return DbType.Int64;
                case TypeCode.SByte:
                    return DbType.SByte;
                case TypeCode.Single:
                    return DbType.Single;
                case TypeCode.String:
                    return DbType.String;
                case TypeCode.UInt16:
                    return DbType.UInt16;
                case TypeCode.UInt32:
                    return DbType.UInt32;
                case TypeCode.UInt64:
                    return DbType.UInt64;
                default:
                    if (propertyType == typeof(byte[]))
                    {
                        return DbType.Binary;
                    }
                    else if (propertyType == typeof(Guid))
                    {
                        return DbType.Guid;
                    }
                    else if (propertyType == typeof(DateTimeOffset))
                    {
                        return DbType.DateTimeOffset;
                    }
                    else if (propertyType == typeof(SqlXml))
                    {
                        return DbType.Xml;
                    }
                    else
                    {
                        return DbType.Object;
                    }
            }
        }

        private static Tout MapReaderToEntity<Tout>(DbDataReader reader)
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

            await _connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync())
            {
                result.Add(MapReaderToEntity<Tout>(reader));
            }
            await _connection.CloseAsync();

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
            AddParameters(command, parameters);

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