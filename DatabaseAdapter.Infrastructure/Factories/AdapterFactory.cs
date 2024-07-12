using DatabaseAdapter.Domain.Enums;
using DatabaseAdapter.Domain.Interfaces;
using DatabaseAdapter.Infrastructure.DataHandlers.AdoNet;
using DatabaseAdapter.Infrastructure.DataHandlers.Cassandra;
using DatabaseAdapter.Infrastructure.DataHandlers.CosmosDB;
using DatabaseAdapter.Infrastructure.DataHandlers.Dapper;
using DatabaseAdapter.Infrastructure.DataHandlers.Elasticsearch;
using DatabaseAdapter.Infrastructure.DataHandlers.EntityFramework;
using DatabaseAdapter.Infrastructure.DataHandlers.MongoDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAdapter.Infrastructure.Factories
{
    public class AdapterFactory 
    {
        private string _connectionString;
        private DatabaseType _databaseType;
        private DataServiceHandlerType _dataServiceHandlerType;
        private IDatabaseAdapter _dataAdapter;

        private readonly Dictionary<(DatabaseType, DataServiceHandlerType), Type> AdapterTypeMappings = new()
        {
            // SQL Server
            {(DatabaseType.SqlServer, DataServiceHandlerType.Dapper), typeof(DapperAdapter)},
            {(DatabaseType.SqlServer, DataServiceHandlerType.EntityFramework), typeof(EntityFrameworkAdapter)},
            {(DatabaseType.SqlServer, DataServiceHandlerType.AdoNet), typeof(AdoNetAdapter)},

            // MySQL
            {(DatabaseType.MySql, DataServiceHandlerType.Dapper), typeof(DapperAdapter)},
            {(DatabaseType.MySql, DataServiceHandlerType.EntityFramework), typeof(EntityFrameworkAdapter)},
            {(DatabaseType.MySql, DataServiceHandlerType.AdoNet), typeof(AdoNetAdapter)},

            // PostgreSQL
            {(DatabaseType.PostgreSql, DataServiceHandlerType.Dapper), typeof(DapperAdapter)},
            {(DatabaseType.PostgreSql, DataServiceHandlerType.EntityFramework), typeof(EntityFrameworkAdapter)},
            {(DatabaseType.PostgreSql, DataServiceHandlerType.AdoNet), typeof(AdoNetAdapter)},

            // SQLite
            {(DatabaseType.SQLite, DataServiceHandlerType.Dapper), typeof(DapperAdapter)},
            {(DatabaseType.SQLite, DataServiceHandlerType.EntityFramework), typeof(EntityFrameworkAdapter)},
            {(DatabaseType.SQLite, DataServiceHandlerType.AdoNet), typeof(AdoNetAdapter)},

            // Oracle
            {(DatabaseType.Oracle, DataServiceHandlerType.Dapper), typeof(DapperAdapter)},
            {(DatabaseType.Oracle, DataServiceHandlerType.EntityFramework), typeof(EntityFrameworkAdapter)},
            {(DatabaseType.Oracle, DataServiceHandlerType.AdoNet), typeof(AdoNetAdapter)},

            // MongoDB
            {(DatabaseType.MongoDB, DataServiceHandlerType.MongoDb), typeof(MongoDbAdapter)},

            // CosmosDB
            {(DatabaseType.CosmosDB, DataServiceHandlerType.CosmosDb), typeof(CosmosDbAdapter)},

            // Cassandra
            {(DatabaseType.Cassandra, DataServiceHandlerType.Cassandra), typeof(CassandraAdapter)},

            // Elasticsearch
            {(DatabaseType.Elasticsearch, DataServiceHandlerType.Elasticsearch), typeof(ElasticsearchAdapter)}
        };

        public void SetConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SetDatabaseType(DatabaseType databaseType)
        {
            _databaseType = databaseType;
        }

        public void SetDataServiceHandlerType(DataServiceHandlerType dataServiceHandlerType)
        {
            _dataServiceHandlerType = dataServiceHandlerType;
        }

        public IDatabaseAdapter BuildAdapter()
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new ArgumentException("Connection string must be set.");

            if (!AdapterTypeMappings.TryGetValue((_databaseType, _dataServiceHandlerType), out var adapterType))
            {
                throw new ArgumentException($"Unsupported combination of database type and data service handler: {_databaseType}, {_dataServiceHandlerType}");
            }

            return (IDatabaseAdapter)Activator.CreateInstance(adapterType, _connectionString, _databaseType);
        }
    }

    
}
