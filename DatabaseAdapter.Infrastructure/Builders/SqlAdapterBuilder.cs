using DatabaseAdapter.DataHandlers.SqlAdapters;
using DatabaseAdapter.Domain.Enums;
using DatabaseAdapter.Domain.Interfaces.SqlAdapters;

namespace DatabaseAdapter.Infrastructure.Factories
{
    public class SqlAdapterBuilder 
    {
        private string? _connectionString;
        private SqlDatabaseType _databaseType;
        private DataServiceHandlerType _dataServiceHandlerType;
        private ISqlAdapter? _dataAdapter;

        private readonly Dictionary<(SqlDatabaseType, DataServiceHandlerType), Type> AdapterTypeMappings = new()
        {
            // SQL Server
            {(SqlDatabaseType.SqlServer, DataServiceHandlerType.Dapper), typeof(DapperAdapter)},
            {(SqlDatabaseType.SqlServer, DataServiceHandlerType.AdoNet), typeof(AdoNetAdapter)},

            // MySQL
            {(SqlDatabaseType.MySql, DataServiceHandlerType.Dapper), typeof(DapperAdapter)},
            {(SqlDatabaseType.MySql, DataServiceHandlerType.AdoNet), typeof(AdoNetAdapter)},

            // PostgreSQL
            {(SqlDatabaseType.PostgreSql, DataServiceHandlerType.Dapper), typeof(DapperAdapter)},
            {(SqlDatabaseType.PostgreSql, DataServiceHandlerType.AdoNet), typeof(AdoNetAdapter)},

            // SQLite
            {(SqlDatabaseType.SQLite, DataServiceHandlerType.Dapper), typeof(DapperAdapter)},
            {(SqlDatabaseType.SQLite, DataServiceHandlerType.AdoNet), typeof(AdoNetAdapter)},

            // Oracle
            {(SqlDatabaseType.Oracle, DataServiceHandlerType.Dapper), typeof(DapperAdapter)},
            {(SqlDatabaseType.Oracle, DataServiceHandlerType.AdoNet), typeof(AdoNetAdapter)},
        };

        public void SetConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SetDatabaseType(SqlDatabaseType databaseType)
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
