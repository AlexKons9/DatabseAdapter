using DatabaseAdapter.Domain.Interfaces.SqlAdapters;
using DatabaseAdapter.Infrastructure.Factories;
using Microsoft.Extensions.Options;
using System.Data;
using WebApplicationTestLib.Entities;
using WebApplicationTestLib.Options;

namespace WebApplicationTestLib.Repositories
{
    public class CategoryRepository
    {
        private readonly ISqlAdapter _sqlAdapter;

        public CategoryRepository(IOptions<DatabaseOptions> options)
        {
            var optionsBuilder = options.Value;
            _sqlAdapter = new SqlAdapterBuilder()
                .SetConnectionString(optionsBuilder.ConnectionString)
                .SetDatabaseType(optionsBuilder.DatabaseType)
                .SetDataServiceHandlerType(optionsBuilder.DataServiceHandlerType)
                .BuildAdapter();
        }

        public async Task<Category> GetByIdAsync(int categoryId, CancellationToken cancellationToken)
        {
                return await _sqlAdapter.FindOneAsync<Category, dynamic>(Queries.Category.GET,
                                                                         new { categoryId },
                                                                         CommandType.Text,
                                                                         cancellationToken);
        }
        public async Task<IEnumerable<Category>> GetAsync(CancellationToken cancellationToken)
        {
            return await _sqlAdapter.FindAsync<Category, dynamic>(Queries.Category.GETALL,
                                                                  new { },
                                                                  CommandType.Text,
                                                                  cancellationToken);
        }

        public async Task CreateAsync(Category category, CancellationToken cancellationToken)
        {
            try
            {
                await _sqlAdapter.BeginTransactionAsync(cancellationToken);
                await _sqlAdapter.SaveAsync<dynamic>(Queries.Category.CREATE,
                                                       new
                                                       {
                                                           categoryName = category.CategoryName,
                                                           description = category.Description,
                                                           picture = category.Picture
                                                       },
                                                       CommandType.Text,
                                                       cancellationToken);
                await _sqlAdapter.CommitTransactionAsync(cancellationToken);
            }
            catch (Exception)
            {
                await _sqlAdapter.RollbackTransactionAsync(cancellationToken);
            }
        }

        public Task UpdateAsync(Category category, CancellationToken cancellationToken)
        {
            return _sqlAdapter.SaveAsync<dynamic>(Queries.Category.UPDATE,
                                                  new
                                                  {
                                                      categoryId = category.CategoryID,
                                                      categoryName = category.CategoryName,
                                                      description = category.Description,
                                                      picture = category.Picture
                                                  },
                                                  CommandType.Text,
                                                  cancellationToken);
        }

        public Task DeleteAsync(int categoryId, CancellationToken cancellationToken)
        {
            return _sqlAdapter.SaveAsync<dynamic>(Queries.Category.DELETE,
                                                  new
                                                  {
                                                      categoryId
                                                  },
                                                  CommandType.Text,
                                                  cancellationToken);
        }
    }
}
