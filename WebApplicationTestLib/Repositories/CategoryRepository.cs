using DatabaseAdapter.Builders;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApplicationTestLib.Entities;


public class CategoryRepository
{
    private const string databaseName = "newdb";
    private const string connectionString = "mongodb://root:example@localhost:27017"; // MongoDB connection string
    private readonly IMongoCollection<Category> _categoryCollection;

    public CategoryRepository()
    {
        // Build the MongoDB adapter using the builder pattern
        var dbAdapter = new MongoDbAdapterBuilder()
            .SetConnectionString(connectionString)
            .SetDatabaseName(databaseName)
            .Build();

        // Ensure that the Category collection exists
        _categoryCollection = dbAdapter.Database.GetCollection<Category>("Category");
    }

    public async Task<Category> GetByIdAsync(string categoryId, CancellationToken cancellationToken)
    {
        try
        {
            var filter = Builders<Category>.Filter.Eq(c => c.Id, categoryId);
            return await _categoryCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // Log or handle other exceptions
            throw new Exception($"An error occurred while retrieving the category: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<Category>> GetAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _categoryCollection.Find(new BsonDocument()).ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // Log or handle other exceptions
            throw new Exception($"An error occurred while retrieving categories: {ex.Message}", ex);
        }
    }

    public async Task CreateAsync(Category category, CancellationToken cancellationToken)
    {
        try
        {
            category.Id = Guid.NewGuid().ToString(); // Set the ID
            await _categoryCollection.InsertOneAsync(category, null, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log or handle exceptions
            throw new Exception($"An error occurred while creating the category: {ex.Message}", ex);
        }
    }

    public async Task UpdateAsync(Category category, CancellationToken cancellationToken)
    {
        try
        {
            var filter = Builders<Category>.Filter.Eq(c => c.Id, category.Id);
            await _categoryCollection.ReplaceOneAsync(filter, category, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            // Log or handle exceptions
            throw new Exception($"An error occurred while updating the category: {ex.Message}", ex);
        }
    }

    public async Task DeleteAsync(string categoryId, CancellationToken cancellationToken)
    {
        try
        {
            var filter = Builders<Category>.Filter.Eq(c => c.Id, categoryId);
            await _categoryCollection.DeleteOneAsync(filter, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log or handle exceptions
            throw new Exception($"An error occurred while deleting the category: {ex.Message}", ex);
        }
    }
}
