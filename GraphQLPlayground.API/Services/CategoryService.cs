using GraphQLPlayground.API.Data;
using GraphQLPlayground.API.ErrorHandling;
using GraphQLPlayground.API.Extensions;
using GraphQLPlayground.API.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace GraphQLPlayground.API.Services;

public class CategoryService : ICategoryService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public CategoryService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public IQueryable<Category> GetAll()
    {
        var context = _contextFactory.CreateDbContext();
        return context.Categories;
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Category> AddCategoryAsync(string name)
    {
        Log.Information("AddCategoryAsync: Iniciando | Name={Name}", name);

        if (string.IsNullOrWhiteSpace(name))
        {
            Log.Warning("AddCategoryAsync: Nome vazio");
            throw new GraphQLException("O nome da categoria nao pode ser vazio.").WithCode(AppErrorCodes.ValidationError);
        }

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var category = new Category { Name = name.Trim() };

            context.Categories.Add(category);
            await context.SaveChangesAsync();

            Log.Information("AddCategoryAsync: Sucesso | Id={Id}, Name={Name}", category.Id, category.Name);
            return category;
        }
        catch (GraphQLException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "AddCategoryAsync: Erro | Name={Name}", name);
            throw;
        }
    }

    public async Task<Category> UpdateCategoryAsync(int id, string name)
    {
        Log.Information("UpdateCategoryAsync: Iniciando | Id={Id}, Name={Name}", id, name);

        if (string.IsNullOrWhiteSpace(name))
        {
            Log.Warning("UpdateCategoryAsync: Nome vazio");
            throw new GraphQLException("O nome da categoria nao pode ser vazio.").WithCode(AppErrorCodes.ValidationError);
        }

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var category = await context.Categories.FindAsync(id);

            if (category is null)
            {
                Log.Warning("UpdateCategoryAsync: Categoria nao encontrada: {Id}", id);
                throw new GraphQLException($"Categoria com ID {id} nao encontrada.").WithCode(AppErrorCodes.CategoryNotFound);
            }

            category.Name = name.Trim();
            await context.SaveChangesAsync();

            Log.Information("UpdateCategoryAsync: Sucesso | Id={Id}", id);
            return category;
        }
        catch (GraphQLException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "UpdateCategoryAsync: Erro | Id={Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        Log.Information("DeleteCategoryAsync: Iniciando | Id={Id}", id);

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var category = await context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category is null)
            {
                Log.Warning("DeleteCategoryAsync: Categoria nao encontrada: {Id}", id);
                throw new GraphQLException($"Categoria com ID {id} nao encontrada.").WithCode(AppErrorCodes.CategoryNotFound);
            }

            if (category.Products.Any())
            {
                Log.Warning("DeleteCategoryAsync: Categoria com produtos vinculados | Id={Id}, ProductsCount={Count}", id, category.Products.Count);
                throw new GraphQLException(
                    $"Nao e possivel excluir a categoria '{category.Name}' pois existem {category.Products.Count} produto(s) vinculado(s).").WithCode(AppErrorCodes.ConstraintViolation);
            }

            context.Categories.Remove(category);
            await context.SaveChangesAsync();

            Log.Information("DeleteCategoryAsync: Sucesso | Id={Id}", id);
            return true;
        }
        catch (GraphQLException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "DeleteCategoryAsync: Erro | Id={Id}", id);
            throw;
        }
    }
}