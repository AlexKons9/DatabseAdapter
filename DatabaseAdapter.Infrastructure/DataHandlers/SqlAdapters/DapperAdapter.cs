﻿using DatabaseAdapter.Domain.Enums;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySqlConnector;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.Common;
using DatabaseAdapter.Domain.Interfaces.SqlAdapters;
using System.Threading;


namespace DatabaseAdapter.DataHandlers.SqlAdapters

{
    public class DapperAdapter : ISqlAdapter
    {
        private readonly DbConnection _connection;
        private DbTransaction? _transaction;
        private readonly DatabaseType _databaseType;

        public DapperAdapter(string connectionString, DatabaseType databaseType)
        {
            _databaseType = databaseType;
            _connection = CreateDbConnection(databaseType, connectionString);
        }

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

        public async Task SaveAsync<Tin>(string query, Tin parameters, CommandType commandType, CancellationToken cancellationToken)
        {
            if (_transaction == null)
            {
                await _connection.OpenAsync(cancellationToken);
                await _connection.ExecuteAsync(query, parameters, commandType: commandType);
                await _connection.CloseAsync();
            }
            else
            {
                await _connection.ExecuteAsync(query, parameters, _transaction, commandType: commandType);
            }
        }

        public async Task<IEnumerable<Tout>> FindAsync<Tout, Tin>(string query, Tin parameters, CommandType commandType, CancellationToken cancellationToken)
        {
            if (_transaction == null)
            {
                await _connection.OpenAsync(cancellationToken);
                var result = await _connection.QueryAsync<Tout>(query, parameters, commandType: commandType);
                await _connection.CloseAsync();
                return result;
            }
            else
            {
                return await _connection.QueryAsync<Tout>(query, parameters, _transaction, commandType: commandType);
            }
        }

        public async Task<Tout?> FindOneAsync<Tout, Tin>(string query, Tin parameters, CommandType commandType, CancellationToken cancellationToken)
        {
            if (_transaction == null)
            {
                await _connection.OpenAsync(cancellationToken);
                var result = await _connection.QueryFirstOrDefaultAsync<Tout>(query, parameters, commandType: commandType);
                await _connection.CloseAsync();
                return result;
            }
            else
            {
                return await _connection.QueryFirstOrDefaultAsync<Tout>(query, parameters, _transaction, commandType: commandType);
            }
        }

        public async Task<Tout?> FindOneAndSaveAsync<Tout, Tin>(string query, Tin parameters, CommandType commandType, CancellationToken cancellationToken)
        {
            Tout? result;
            if (_transaction == null)
            {
                await _connection.OpenAsync(cancellationToken);
                result = await _connection.QueryFirstOrDefaultAsync<Tout>(query, parameters, commandType: commandType);
                await _connection.ExecuteAsync(query, parameters, commandType: commandType);
                await _connection.CloseAsync();
            }
            else
            {
                result = await _connection.QueryFirstOrDefaultAsync<Tout>(query, parameters, _transaction, commandType: commandType);
                await _connection.ExecuteAsync(query, parameters, _transaction, commandType: commandType);
            }
            return result;
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
    }
}

