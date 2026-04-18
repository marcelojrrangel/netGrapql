using GraphQLPlayground.API.Services;
using HotChocolate;
using Microsoft.EntityFrameworkCore;

namespace GraphQLPlayground.API.Tests;

public class ProductServiceTests
{
    [Fact]
    public async Task AddProductAsync_WithInvalidCategory_ShouldThrowGraphQLException()
    {
        var factory = await TestDbContextFactory.CreateInitializedAsync();
        var service = new ProductService(factory);

        await Assert.ThrowsAsync<GraphQLException>(() => service.AddProductAsync("Produto X", 120, 999));
    }

    [Fact]
    public async Task UpdateProductAsync_ShouldUpdateNameAndPrice()
    {
        var factory = await TestDbContextFactory.CreateInitializedAsync();
        var service = new ProductService(factory);

        var updated = await service.UpdateProductAsync(1, "Smartphone Pro", 1299.99m);

        Assert.Equal(1, updated.Id);
        Assert.Equal("Smartphone Pro", updated.Name);
        Assert.Equal(1299.99m, updated.Price);

        await using var context = await factory.CreateDbContextAsync();
        var saved = await context.Products.FindAsync(1);

        Assert.NotNull(saved);
        Assert.Equal("Smartphone Pro", saved!.Name);
        Assert.Equal(1299.99m, saved.Price);
    }

    [Fact]
    public async Task ApplyDiscountAsync_ShouldReduceProductPrices()
    {
        var factory = await TestDbContextFactory.CreateInitializedAsync();
        var service = new ProductService(factory);

        await using var beforeContext = await factory.CreateDbContextAsync();
        var before = await beforeContext.Products
            .Where(p => p.Id == 1 || p.Id == 2)
            .OrderBy(p => p.Id)
            .Select(p => new { p.Id, p.Price })
            .ToListAsync();

        var result = await service.ApplyDiscountAsync([1, 2], 10);

        Assert.Equal(2, result.Count);

        await using var afterContext = await factory.CreateDbContextAsync();
        var after = await afterContext.Products
            .Where(p => p.Id == 1 || p.Id == 2)
            .OrderBy(p => p.Id)
            .Select(p => new { p.Id, p.Price })
            .ToListAsync();

        Assert.Equal(before[0].Price - (before[0].Price * 0.10m), after[0].Price);
        Assert.Equal(before[1].Price - (before[1].Price * 0.10m), after[1].Price);
    }

    [Fact]
    public async Task DeleteProductAsync_ShouldRemoveProduct()
    {
        var factory = await TestDbContextFactory.CreateInitializedAsync();
        var service = new ProductService(factory);

        var deleted = await service.DeleteProductAsync(3);

        Assert.True(deleted);

        await using var context = await factory.CreateDbContextAsync();
        var saved = await context.Products.FindAsync(3);

        Assert.Null(saved);
    }
}

