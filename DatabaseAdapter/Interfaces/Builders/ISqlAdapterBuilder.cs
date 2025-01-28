using DatabaseAdapter.Domain.Enums;
using DatabaseAdapter.Domain.Interfaces.SqlAdapters;

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
