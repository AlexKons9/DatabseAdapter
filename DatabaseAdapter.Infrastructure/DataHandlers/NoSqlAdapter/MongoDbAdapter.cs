using MongoDB.Driver;


namespace DatabaseAdapter.DataHandlers.NoSqlAdapter
{
    public class MongoDbAdapter
    {
        private readonly IMongoDatabase _database;

        public MongoDbAdapter(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoDatabase GetDatabase()
        {
            return _database;
        }
    }
}
