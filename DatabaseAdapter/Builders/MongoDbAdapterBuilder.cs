using DatabaseAdapter.DataHandlers.NoSqlAdapter;
using DatabaseAdapter.Interfaces.Builders;

namespace DatabaseAdapter.Builders
{
    public class MongoDbAdapterBuilder : IMongoDbAdapterBuilder
    {
        private string? _connectionString;
        private string? _database;
        private MongoDbAdapter _adapter;

        public MongoDbAdapterBuilder()
        {
            _adapter = MongoDbAdapter.Create();
        }

        public IMongoDbAdapterBuilder SetConnectionString(string connectionString)
        {
            _adapter.SetConnectionString(connectionString);
            return this;
        }

        public IMongoDbAdapterBuilder SetDatabaseName(string databaseName)
        {
            _adapter.SetDatabaseName(databaseName);
            return this;
        }

        public MongoDbAdapter Build()
        {
            return _adapter;
        }
    }
}
