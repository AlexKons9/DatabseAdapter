using DatabaseAdapter.DataHandlers.NoSqlAdapter;

namespace DatabaseAdapter.Interfaces.Builders
{
    public interface IMongoDbAdapterBuilder
    {
        public IMongoDbAdapterBuilder SetConnectionString(string connectionString);
        public IMongoDbAdapterBuilder SetDatabaseName(string databaseName);
        public MongoDbAdapter Build();
    }
}
