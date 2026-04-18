using GraphQLPlayground.API.Models;

namespace GraphQLPlayground.API.Services;

public interface IProductService
{
    IQueryable<Product> GetAll();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> AddProductAsync(string name, decimal price, int categoryId);
    Task<Product> UpdateProductAsync(int id, string? name, decimal? price);
    Task<Product> UpdateProductCategoryAsync(int productId, int newCategoryId);
    Task<bool> DeleteProductAsync(int id);
    Task<List<Product>> ApplyDiscountAsync(List<int> productIds, decimal discountPercentage);
}

