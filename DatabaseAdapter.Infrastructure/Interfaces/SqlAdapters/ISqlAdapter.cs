using System.Data;

namespace DatabaseAdapter.Domain.Interfaces.SqlAdapters
{
    public interface ISqlAdapter
    {
        Task<IEnumerable<Tout>> FindAsync<Tout, Tin>(string query, Tin parameters, CommandType commandType, CancellationToken cancellationToken);
        Task<Tout> FindOneAsync<Tout, Tin>(string query, Tin parameters, CommandType commandType, CancellationToken cancellationToken);
        Task<IEnumerable<Tout>> SaveAndFindAsync<Tout, Tin>(string query, Tin parameters, CommandType commandType, CancellationToken cancellationToken);
        Task<Tout> SaveAndFindOneAsync<Tout, Tin>(string query, Tin parameters, CommandType commandType, CancellationToken cancellationToken);
        Task SaveAsync<Tin>(string query, Tin parameters, CommandType commandType, CancellationToken cancellationToken);
        Task RollbackTransactionAsync(CancellationToken cancellationToken);
        Task CommitTransactionAsync(CancellationToken cancellationToken);
        Task BeginTransactionAsync(CancellationToken cancellationToken);
    }
}
