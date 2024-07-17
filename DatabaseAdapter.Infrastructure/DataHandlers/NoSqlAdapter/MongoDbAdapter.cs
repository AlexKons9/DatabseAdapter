using MongoDB.Driver;


namespace DatabaseAdapter.DataHandlers.NoSqlAdapter
{
    public class MongoDbAdapter
    {
        public IMongoDatabase? Database { get; private set; }
        public MongoClient? MongoClient { get; private set; }

        private MongoDbAdapter() { }

        public static MongoDbAdapter Create()
        {
            return new MongoDbAdapter(); 
        }

        public void SetConnectionString(string connectionString)
        {
            MongoClient = new MongoClient(connectionString);
        }

        public void SetDatabase(string databaseName)
        {
            Database = MongoClient.GetDatabase(databaseName);
        }
    }
}
