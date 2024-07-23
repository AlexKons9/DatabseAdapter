using DatabaseAdapter.Domain.Enums;
using DatabaseAdapter.Domain.Interfaces.SqlAdapters;
using MongoDB.Driver.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAdapter.Interfaces.Builders
{
    public interface ISqlAdapterBuilder
    {
        ISqlAdapterBuilder SetConnectionString(string connectionString);

        ISqlAdapterBuilder SetDatabaseType(DatabaseType databaseType);

        ISqlAdapterBuilder SetDataServiceHandlerType(DataServiceHandlerType dataServiceHandlerType);

        public ISqlAdapter BuildAdapter();
    }
}
