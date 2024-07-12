using DatabaseAdapter.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAdapter.Domain.Interfaces
{
    public interface IDatabaseAdapter
    {
        Task SaveAsync<Tin>(string query, Tin parameters, CommandType commandType);
        Task<IEnumerable<Tout>> FindAsync<Tout, Tin>(string query, Tin parameters, CommandType commandType);
        Task<Tout> FindOneAndSaveAsync<Tout, Tin>(string query, Tin parameters, CommandType commandType);
        Task<Tout> FindOneAsync<Tout, Tin>(string query, Tin parameters, CommandType commandType);
        Task RollbackTransactionAsync();
        Task CommitTransactionAsync();
        Task BeginTransactionAsync();
    }
}
