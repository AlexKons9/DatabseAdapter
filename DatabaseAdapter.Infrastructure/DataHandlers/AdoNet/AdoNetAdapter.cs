using DatabaseAdapter.Domain.Enums;
using DatabaseAdapter.Domain.Interfaces;
using DatabaseAdapter.Infrastructure.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySqlConnector;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.Common;

namespace DatabaseAdapter.Infrastructure.DataHandlers.AdoNet
{
    /// <summary>
    /// Provides an implementation of <see cref="IDatabaseAdapter"/> using ADO.NET for various database types.
    /// </summary>
    public class AdoNetAdapter : IDatabaseAdapter
    {
        private readonly DbConnection _connection;
        private DbTransaction? _transaction;
        private readonly DatabaseType _databaseType;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetAdapter"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to the database.</param>
        /// <param name="databaseType">The type of the database.</param>
        public AdoNetAdapter(string connectionString, DatabaseType databaseType)
        {
            _databaseType = databaseType;
            _connection = CreateDbConnection(databaseType, connectionString);
        }

        /// <summary>
        /// Creates a database connection based on the specified database type.
        /// </summary>
        /// <param name="databaseType">The type of the database.</param>
        /// <param name="connectionString">The connection string to the database.</param>
        /// <returns>A <see cref="DbConnection"/> instance for the specified database.</returns>
        private static DbConnection CreateDbConnection(DatabaseType databaseType, string connectionString)
        {
            return databaseType switch
            {
                DatabaseType.SqlServer => new SqlConnection(connectionString),
                DatabaseType.MySql => new MySqlConnection(connectionString),
                DatabaseType.PostgreSql => new NpgsqlConnection(connectionString),
                DatabaseType.SQLite => new SqliteConnection(connectionString),
                DatabaseType.Oracle => new OracleConnection(connectionString),
                _ => throw new NotSupportedException($"Database type {databaseType} is not supported.")
            };
        }

        /// <summary>
        /// Saves data to the database using the specified query and parameters.
        /// </summary>
        /// <typeparam name="Tin">The type of the parameters to be passed to the query.</typeparam>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="parameters">The parameters for the SQL query.</param>
        /// <param name="commandType">The type of command (e.g., Text, StoredProcedure).</param>
        /// <returns>A task representing the asynchronous save operation.</returns>
        public async Task SaveAsync<Tin>(string query, Tin parameters, CommandType commandType)
        {
            using var command = _connection.CreateCommand();
            command.CommandText = query;
            command.CommandType = commandType;
            command.Transaction = _transaction;
            AddParameters(command, parameters);

            if (_transaction == null)
            {
                await _connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
                await _connection.CloseAsync();
            }
            else
            {
                await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Finds multiple records from the database using the specified query and parameters.
        /// </summary>
        /// <typeparam name="Tout">The type of the expected result.</typeparam>
        /// <typeparam name="Tin">The type of the parameters to be passed to the query.</typeparam>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="parameters">The parameters for the SQL query.</param>
        /// <param name="commandType">The type of command.</param>
        /// <returns>A task that represents the asynchronous operation, containing an enumerable of the results.</returns>
        public async Task<IEnumerable<Tout>> FindAsync<Tout, Tin>(string query, Tin parameters, CommandType commandType)
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
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Add(MapReaderToEntity<Tout>(reader));
                }
                await _connection.CloseAsync();
            }
            else
            {
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Add(MapReaderToEntity<Tout>(reader));
                }
            }

            return result;
        }

        /// <summary>
        /// Finds a single record from the database using the specified query and parameters.
        /// </summary>
        /// <typeparam name="Tout">The type of the expected result.</typeparam>
        /// <typeparam name="Tin">The type of the parameters to be passed to the query.</typeparam>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="parameters">The parameters for the SQL query.</param>
        /// <param name="commandType">The type of command.</param>
        /// <returns>A task that represents the asynchronous operation, containing the result or null if not found.</returns>
        public async Task<Tout?> FindOneAsync<Tout, Tin>(string query, Tin parameters, CommandType commandType)
        {
            using var command = _connection.CreateCommand();
            command.CommandText = query;
            command.CommandType = commandType;
            command.Transaction = _transaction;
            AddParameters(command, parameters);

            if (_transaction == null)
            {
                await _connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                Tout? result = default;
                if (await reader.ReadAsync())
                {
                    result = MapReaderToEntity<Tout>(reader);
                }
                await _connection.CloseAsync();
                return result;
            }
            else
            {
                using var reader = await command.ExecuteReaderAsync();
                Tout? result = default;
                if (await reader.ReadAsync())
                {
                    result = MapReaderToEntity<Tout>(reader);
                }
                return result;
            }
        }

        /// <summary>
        /// Finds a single record and saves it using the specified query and parameters.
        /// </summary>
        /// <typeparam name="Tout">The type of the expected result.</typeparam>
        /// <typeparam name="Tin">The type of the parameters to be passed to the query.</typeparam>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="parameters">The parameters for the SQL query.</param>
        /// <param name="commandType">The type of command.</param>
        /// <returns>A task that represents the asynchronous operation, containing the result or null if not found.</returns>
        public async Task<Tout?> FindOneAndSaveAsync<Tout, Tin>(string query, Tin parameters, CommandType commandType)
        {
            using var command = _connection.CreateCommand();
            command.CommandText = query;
            command.CommandType = commandType;
            command.Transaction = _transaction;
            AddParameters(command, parameters);

            if (_transaction == null)
            {
                await _connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                Tout? result = default;
                if (await reader.ReadAsync())
                {
                    result = MapReaderToEntity<Tout>(reader);
                }
                await command.ExecuteNonQueryAsync();
                await _connection.CloseAsync();
                return result;
            }
            else
            {
                using var reader = await command.ExecuteReaderAsync();
                Tout? result = default;
                if (await reader.ReadAsync())
                {
                    result = MapReaderToEntity<Tout>(reader);
                }
                await command.ExecuteNonQueryAsync();
                return result;
            }
        }

        /// <summary>
        /// Begins a new transaction on the database connection.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task BeginTransactionAsync()
        {
            if (_connection.State == ConnectionState.Closed)
            {
                await _connection.OpenAsync();
            }

            _transaction = await _connection.BeginTransactionAsync();
        }

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _connection.CloseAsync();
                _transaction = null;
            }
        }

        /// <summary>
        /// Rolls back the current transaction.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _connection.CloseAsync();
                _transaction = null;
            }
        }

        /// <summary>
        /// Adds parameters to the database command from the specified parameters object.
        /// </summary>
        /// <typeparam name="T">The type of the parameters object.</typeparam>
        /// <param name="command">The database command to which parameters are added.</param>
        /// <param name="parameters">The parameters object containing values.</param>
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

        /// <summary>
        /// Maps a <see cref="DbDataReader"/> row to an entity of type <typeparamref name="Tout"/>.
        /// </summary>
        /// <typeparam name="Tout">The type of the entity to map to.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> instance containing the data.</param>
        /// <returns>An instance of type <typeparamref name="Tout"/> populated with data from the reader.</returns>
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
