using DatabaseAdapter.DataHandlers.NoSqlAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAdapter.Interfaces.Builders
{
    public interface ICosmosDbAdapterBuilder
    {
        public ICosmosDbAdapterBuilder SetConnectionString(string connectionString);
        public ICosmosDbAdapterBuilder SetDatabase(string database);
        public CosmosDbAdapter Build();
    }
}
