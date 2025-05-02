using BatchProcessing.Domain.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace BatchProcessing.Domain;

/// <summary>
/// Represents the application context for batch processing, integrating both SQL and MongoDB databases.
/// </summary>
public sealed class ApplicationContext : DbContext
{
    private readonly IMongoDatabase _mongoDatabase;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationContext"/> class.
    /// </summary>
    /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
    /// <param name="mongoDatabase">The MongoDB database instance.</param>
    public ApplicationContext(DbContextOptions options, IMongoDatabase mongoDatabase) : base(options)
    {
        _mongoDatabase = mongoDatabase;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ApplicationContext"/> using the specified MongoDB client and database.
    /// </summary>
    /// <param name="mongoClient">The MongoDB client.</param>
    /// <param name="mongoDatabase">The MongoDB database instance.</param>
    /// <returns>A new instance of <see cref="ApplicationContext"/>.</returns>
    public static ApplicationContext Create(IMongoClient mongoClient, IMongoDatabase mongoDatabase) => new ApplicationContext(
        new DbContextOptionsBuilder<ApplicationContext>()
            .UseMongoDB(mongoClient, "mongoDb")
            .Options, mongoDatabase);

    /// <summary>
    /// Gets or sets the batch processes.
    /// </summary>
    public DbSet<BatchProcess> BatchProcesses => Set<BatchProcess>();

    /// <summary>
    /// Gets or sets the batch process items.
    /// </summary>
    public DbSet<BatchProcessItem> BatchProcessItems => Set<BatchProcessItem>();

    /// <summary>
    /// Inserts a collection of <see cref="BatchProcessItem"/> into the MongoDB database in bulk.
    /// </summary>
    /// <param name="items">The collection of <see cref="BatchProcessItem"/> to insert.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task BulkInsert(IEnumerable<BatchProcessItem> items)
    {
        var entities = _mongoDatabase.GetCollection<BatchProcessItem>(nameof(BatchProcessItem));
        return entities.InsertManyAsync(items);
    }

    /// <summary>
    /// Configures the database (and other options) to be used for this context.
    /// </summary>
    /// <param name="optionsBuilder">A builder used to create or modify options for this context.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Configuration logic here
    }

    /// <summary>
    /// Configures the model that was discovered by convention from the entity types
    /// exposed in <see cref="DbSet{TEntity}"/> properties on your derived context.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BatchProcess>().ToCollection(nameof(BatchProcess));
        modelBuilder.Entity<BatchProcessItem>().ToCollection(nameof(BatchProcessItem));
    }
}
