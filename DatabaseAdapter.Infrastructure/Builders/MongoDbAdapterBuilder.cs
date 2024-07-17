using DatabaseAdapter.DataHandlers.NoSqlAdapter;
using DatabaseAdapter.Interfaces.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAdapter.Builders
{
    public class MongoDbAdapterBuilder : IMongoDbAdapterBuilder
    {
        private string? _connectionString;
        private string? _database;
        private MongoDbAdapter _adapter;

        public MongoDbAdapterBuilder()
        {
            _adapter = MongoDbAdapter.Create();
        }

        public IMongoDbAdapterBuilder SetConnectionString(string connectionString)
        {
            _adapter.SetConnectionString(connectionString);
            return this;
        }

        public IMongoDbAdapterBuilder SetDatabase(string databaseName)
        {
            _adapter.SetDatabase(databaseName);
            return this;
        }

        public MongoDbAdapter Build()
        {
            return _adapter;
        }
    }
}
