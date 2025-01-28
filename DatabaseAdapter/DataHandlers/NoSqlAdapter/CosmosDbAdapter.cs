using Microsoft.Azure.Cosmos;

namespace DatabaseAdapter.DataHandlers.NoSqlAdapter
{
    public class CosmosDbAdapter
    {
        public Database Database { get; private set; }
        public CosmosClient Client { get; private set; }


        private CosmosDbAdapter() { }

        public static CosmosDbAdapter Create()
        {
            return new CosmosDbAdapter();
        }

        public void SetConnectionString(string connectionString, CosmosClientOptions? options = null)
        {
            Client = new CosmosClient(connectionString, options);
        }

        public void SetCredentialsForConnection(string accountEndpoint, string authKeyOrResourceToken, CosmosClientOptions? options = null)
        {
            Client = new CosmosClient(accountEndpoint, authKeyOrResourceToken, options);
        }

        public async Task SetDatabaseName(string databaseName)
        {
            Database = await Client.CreateDatabaseIfNotExistsAsync(databaseName);
        }

    }
}
