using DatabaseAdapter.DataHandlers.SqlAdapters;
using DatabaseAdapter.Domain.Enums;
using DatabaseAdapter.Domain.Interfaces.SqlAdapters;
using DatabaseAdapter.Interfaces.Builders;

namespace DatabaseAdapter.Infrastructure.Factories
{
    public class SqlAdapterBuilder : ISqlAdapterBuilder
    {
        private string? _connectionString;
        private DatabaseType _databaseType;
        private DataServiceHandlerType _dataServiceHandlerType;
        private ISqlAdapter? _dataAdapter;

        private readonly Dictionary<(DatabaseType, DataServiceHandlerType), Type> AdapterTypeMappings = new()
        {
            // SQL Server
            {(DatabaseType.SqlServer, DataServiceHandlerType.Dapper), typeof(DapperAdapter)},
            {(DatabaseType.SqlServer, DataServiceHandlerType.AdoNet), typeof(AdoNetAdapter)},

            // MySQL
            {(DatabaseType.MySql, DataServiceHandlerType.Dapper), typeof(DapperAdapter)},
            {(DatabaseType.MySql, DataServiceHandlerType.AdoNet), typeof(AdoNetAdapter)},

            // PostgreSQL
            {(DatabaseType.PostgreSql, DataServiceHandlerType.Dapper), typeof(DapperAdapter)},
            {(DatabaseType.PostgreSql, DataServiceHandlerType.AdoNet), typeof(AdoNetAdapter)},

            // SQLite
            {(DatabaseType.SQLite, DataServiceHandlerType.Dapper), typeof(DapperAdapter)},
            {(DatabaseType.SQLite, DataServiceHandlerType.AdoNet), typeof(AdoNetAdapter)},

            // Oracle
            {(DatabaseType.Oracle, DataServiceHandlerType.Dapper), typeof(DapperAdapter)},
            {(DatabaseType.Oracle, DataServiceHandlerType.AdoNet), typeof(AdoNetAdapter)},
        };

        public ISqlAdapterBuilder SetConnectionString(string connectionString)
        {
            _connectionString = connectionString;
            return this;
        }

        public ISqlAdapterBuilder SetDatabaseType(DatabaseType databaseType)
        {
            _databaseType = databaseType;
            return this;
        }

        public ISqlAdapterBuilder SetDataServiceHandlerType(DataServiceHandlerType dataServiceHandlerType)
        {
            _dataServiceHandlerType = dataServiceHandlerType;
            return this;
        }

        public ISqlAdapter? BuildAdapter()
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new ArgumentException("Connection string must be set.");

            if (!AdapterTypeMappings.TryGetValue((_databaseType, _dataServiceHandlerType), out var adapterType))
            {
                throw new ArgumentException($"Unsupported combination of database type and data service handler: {_databaseType}, {_dataServiceHandlerType}");
            }

            _dataAdapter = Activator.CreateInstance(adapterType, _connectionString, _databaseType) as ISqlAdapter;
            return _dataAdapter;
        }
    }

    
}
