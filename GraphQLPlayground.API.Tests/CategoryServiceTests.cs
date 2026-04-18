using GraphQLPlayground.API.Services;
using HotChocolate;
using Microsoft.EntityFrameworkCore;

namespace GraphQLPlayground.API.Tests;

public class CategoryServiceTests
{
    [Fact]
    public async Task AddCategoryAsync_ShouldPersistCategory()
    {
        var factory = await TestDbContextFactory.CreateInitializedAsync();
        var service = new CategoryService(factory);

        var created = await service.AddCategoryAsync("Gaming");

        Assert.True(created.Id > 0);
        Assert.Equal("Gaming", created.Name);

        await using var context = await factory.CreateDbContextAsync();
        var saved = await context.Categories.FirstOrDefaultAsync(c => c.Id == created.Id);

        Assert.NotNull(saved);
        Assert.Equal("Gaming", saved!.Name);
    }

    [Fact]
    public async Task DeleteCategoryAsync_WithLinkedProducts_ShouldThrowGraphQLException()
    {
        var factory = await TestDbContextFactory.CreateInitializedAsync();
        var service = new CategoryService(factory);

        await Assert.ThrowsAsync<GraphQLException>(() => service.DeleteCategoryAsync(1));
    }
}

