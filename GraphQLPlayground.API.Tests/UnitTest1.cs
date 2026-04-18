using GraphQLPlayground.API.Data;
using GraphQLPlayground.API.Models;
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

        var electronics = new Category { Id = 1, Name = "Electronics" };
        var gaming = new Category { Id = 2, Name = "Gaming" };

        context.Categories.AddRange(electronics, gaming);

        context.Products.AddRange(
            new Product { Id = 1, Name = "Smartphone", Price = 1000m, CategoryId = electronics.Id },
            new Product { Id = 2, Name = "Notebook", Price = 2000m, CategoryId = electronics.Id },
            new Product { Id = 3, Name = "Mouse", Price = 100m, CategoryId = electronics.Id }
        );

        await context.SaveChangesAsync();

        return factory;
    }
}
