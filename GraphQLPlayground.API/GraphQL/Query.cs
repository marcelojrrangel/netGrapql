using GraphQLPlayground.API.Models;
using GraphQLPlayground.API.Services;

namespace GraphQLPlayground.API.GraphQL;

public class Query
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Product> GetProducts([Service] IProductService productService) 
        => productService.GetAll();

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Category> GetCategories([Service] ICategoryService categoryService) 
        => categoryService.GetAll();

    public async Task<Category?> GetCategoryByIdAsync(
        int id,
        [Service] ICategoryService categoryService)
    {
        return await categoryService.GetByIdAsync(id);
    }
}
