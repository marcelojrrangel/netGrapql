using GraphQLPlayground.API.Models;

namespace GraphQLPlayground.API.Services;

public interface ICategoryService
{
    IQueryable<Category> GetAll();
    Task<Category?> GetByIdAsync(int id);
    Task<Category> AddCategoryAsync(string name);
    Task<Category> UpdateCategoryAsync(int id, string name);
    Task<bool> DeleteCategoryAsync(int id);
}

