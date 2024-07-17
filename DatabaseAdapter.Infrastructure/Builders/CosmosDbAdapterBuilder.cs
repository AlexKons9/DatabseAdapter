using DatabaseAdapter.DataHandlers.NoSqlAdapter;
using DatabaseAdapter.Interfaces.Builders;


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

        public ICosmosDbAdapterBuilder SetConnectionString(string connectionString)
        {
            _adapter.SetConnectionString(connectionString);
            return this;
        }

        public ICosmosDbAdapterBuilder SetDatabase(string database)
        {
            _adapter.SetDatabase(database);
            return this;
        }

        public CosmosDbAdapter Build()
        {
            return _adapter;
        }

    }
}
