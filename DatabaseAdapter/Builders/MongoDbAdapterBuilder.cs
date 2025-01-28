using DatabaseAdapter.DataHandlers.NoSqlAdapter;

namespace DatabaseAdapter.Builders
{
    public class MongoDbAdapterBuilder
    {
        private string? _connectionString;
        private string? _database;
        private MongoDbAdapter _adapter;

        public MongoDbAdapterBuilder()
        {
            _adapter = MongoDbAdapter.Create();
        }

        public MongoDbAdapterBuilder SetConnectionString(string connectionString)
        {
            _adapter.SetConnectionString(connectionString);
            return this;
        }

        public MongoDbAdapterBuilder SetDatabaseName(string databaseName)
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
