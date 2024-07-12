using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAdapter.Domain.Enums
{
    public enum DatabaseType
    {
        SqlServer,
        MySql,
        MariaDB,
        PostgreSql,
        SQLite,
        Oracle,
        MongoDB,
        CosmosDB,
        Cassandra,
        Elasticsearch
    }
}
