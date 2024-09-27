using DatabaseAdapter.DataHandlers.NoSqlAdapter;
using Microsoft.Azure.Cosmos;

namespace DatabaseAdapter.Interfaces.Builders
{
    public interface ICosmosDbAdapterBuilder
    {
        ICosmosDbAdapterBuilder SetConnectionString(string connectionString, CosmosClientOptions? options = null);
        ICosmosDbAdapterBuilder SetCredentialsForConnection(string accountEndpoint, string authKeyOrResourceToken, CosmosClientOptions? options = null);
        Task<ICosmosDbAdapterBuilder> SetDatabaseNameAsync(string database);
        Task<CosmosDbAdapter> BuildAsync();
    }
}
