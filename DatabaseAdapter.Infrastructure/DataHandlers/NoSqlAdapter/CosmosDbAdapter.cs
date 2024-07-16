using Microsoft.Azure.Cosmos;

namespace DatabaseAdapter.DataHandlers.NoSqlAdapter
{
    public class CosmosDbAdapter
    {
        private readonly Database _database;

        public CosmosDbAdapter(string connectionString, string databaseName)
        {
            var client = new CosmosClient(connectionString);
            _database = client.GetDatabase(databaseName);

        }

        public Database GetDatabase()
        {
            return _database;
        }
    }
}
