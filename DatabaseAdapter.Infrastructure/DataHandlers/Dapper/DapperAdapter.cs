using DatabaseAdapter.Domain.Enums;
using DatabaseAdapter.Domain.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySqlConnector;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace DatabaseAdapter.Infrastructure.DataHandlers.Dapper
{
    public class DapperAdapter : IDatabaseAdapter
    {
        public Task BeginTransactionAsync()
        {
            throw new NotImplementedException();
        }

        public Task CommitTransactionAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Tout>> FindAsync<Tout, Tin>(string query, Tin parameters, CommandType commandType)
        {
            throw new NotImplementedException();
        }

        public Task<Tout> FindOneAndSaveAsync<Tout, Tin>(string query, Tin parameters, CommandType commandType)
        {
            throw new NotImplementedException();
        }

        public Task<Tout> FindOneAsync<Tout, Tin>(string query, Tin parameters, CommandType commandType)
        {
            throw new NotImplementedException();
        }

        public Task RollbackTransactionAsync()
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync<Tin>(string query, Tin parameters, CommandType commandType)
        {
            throw new NotImplementedException();
        }
    }
}
