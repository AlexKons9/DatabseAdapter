using DatabaseAdapter.DataHandlers.NoSqlAdapter;
using DatabaseAdapter.Interfaces.Builders;
using Microsoft.Azure.Cosmos;


namespace DatabaseAdapter.Infrastructure.Factories
{
    public class CosmosDbAdapterBuilder : ICosmosDbAdapterBuilder
    {
        private string? _connectionString;
        private string? _database;
        private CosmosDbAdapter _adapter;

        public CosmosDbAdapterBuilder()
        {
            _adapter = CosmosDbAdapter.Create();
        }

        public ICosmosDbAdapterBuilder SetConnectionString(string connectionString, CosmosClientOptions? options = null)
        {
            _adapter.SetConnectionString(connectionString, options);
            return this;
        }

        public ICosmosDbAdapterBuilder SetCredentialsForConnection(string accountEndpoint, string authKeyOrResourceToken, CosmosClientOptions? options = null)
        {
            _adapter.SetCredentialsForConnection(accountEndpoint, authKeyOrResourceToken, options);
            return this;
        }

        public async Task<ICosmosDbAdapterBuilder> SetDatabaseNameAsync(string database)
        {
            await _adapter.SetDatabaseName(database);
            return this;
        }

        public Task<CosmosDbAdapter> BuildAsync()
        {
            return Task.FromResult(_adapter);
        }

    }
}
