using GraphQLPlayground.API.Data;
using Microsoft.EntityFrameworkCore;

namespace GraphQLPlayground.API.Tests;

internal sealed class TestDbContextFactory(DbContextOptions<AppDbContext> options) : IDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext() => new(options);

    public Task<AppDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(CreateDbContext());

    public static async Task<TestDbContextFactory> CreateInitializedAsync()
    {
        var dbName = $"graphql-tests-{Guid.NewGuid():N}";
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        var factory = new TestDbContextFactory(options);

        await using var context = await factory.CreateDbContextAsync();
        await context.Database.EnsureCreatedAsync();

        return factory;
    }
}
