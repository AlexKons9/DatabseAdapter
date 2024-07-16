using DatabaseAdapter.DataHandlers.NoSqlAdapter;
using DatabaseAdapter.Domain.Enums;
using DatabaseAdapter.Interfaces.NoSqlAdapters;


namespace DatabaseAdapter.Infrastructure.Factories
{
    public class NoSqlAdapterFactory
    {
        private string? _connectionString;
        private DatabaseType _databaseType;


        private readonly Dictionary<DatabaseType, Type> AdapterTypeMappings = new()
        {
            { DatabaseType.MongoDB, typeof(MongoDbAdapter) },
            { DatabaseType.CosmosDB, typeof(CosmosDbAdapter) }
        };

        public void SetConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SetDatabaseType(DatabaseType databaseType)
        {
            _databaseType = databaseType;
        }

        public object? BuildAdapter()
        {

            if (string.IsNullOrEmpty(_connectionString))
                throw new ArgumentException("Connection string must be set.");


            if (!AdapterTypeMappings.TryGetValue(_databaseType, out var type))
            {
                throw new ArgumentException($"Unsupported combination of database type : {_databaseType}");
            }

            var _dataAdapter = Activator.CreateInstance(type, _connectionString, _databaseType);
            return _dataAdapter;
        }
    }
}
