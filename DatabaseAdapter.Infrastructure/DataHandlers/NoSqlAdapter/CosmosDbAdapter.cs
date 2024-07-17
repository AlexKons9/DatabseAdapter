using Microsoft.Azure.Cosmos;

namespace DatabaseAdapter.DataHandlers.NoSqlAdapter
{
    public class CosmosDbAdapter
    {
        public Database? Database { get; private set; }
        public CosmosClient? Client { get; private set; }


        private CosmosDbAdapter() { }

        public static CosmosDbAdapter Create()
        {
            return new CosmosDbAdapter(); 
        }

        public void SetConnectionString (string connectionString)
        {
            Client = new CosmosClient(connectionString);
        }

        public Database SetDatabase(string databaseName)
        {
            return Database = Client.GetDatabase(databaseName); ;
        }

    }
}
