using DatabaseAdapter.Domain.Enums;

namespace WebApplicationTestLib.Options
{
    public class DatabaseOptions
    {
        public string ConnectionString { get; set; }
        public DatabaseType DatabaseType { get; set; }
        public DataServiceHandlerType DataServiceHandlerType { get; set; }
    }
}
