using DatabaseAdapter.DataHandlers.NoSqlAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAdapter.Interfaces.Builders
{
    public interface IMongoDbAdapterBuilder
    {
        public IMongoDbAdapterBuilder SetConnectionString(string connectionString);
        public IMongoDbAdapterBuilder SetDatabase(string databaseName);
        public MongoDbAdapter Build();
    }
}
