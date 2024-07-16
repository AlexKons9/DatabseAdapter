using DatabaseAdapter.DataHandlers.SqlAdapters;
using DatabaseAdapter.Domain.Enums;
using DatabaseAdapter.Domain.Interfaces.SqlAdapters;

namespace DatabaseAdapter.Infrastructure.Factories
{
    public class SqlAdapterFactory 
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
