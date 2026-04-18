using GraphQLPlayground.API.Data;
using GraphQLPlayground.API.ErrorHandling;
using GraphQLPlayground.API.Extensions;
using GraphQLPlayground.API.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace GraphQLPlayground.API.Services;

public class ProductService : IProductService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public ProductService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Products.ToListAsync();
    }

    public IQueryable<Product> GetAll()
    {
        var context = _contextFactory.CreateDbContext();
        return context.Products;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Products.FindAsync(id);
    }

    public async Task<Product> AddProductAsync(string name, decimal price, int categoryId)
    {
        Log.Information("AddProductAsync: Iniciando | Name={Name}, Price={Price}, CategoryId={CategoryId}", name, price, categoryId);

        if (string.IsNullOrWhiteSpace(name))
        {
            Log.Warning("AddProductAsync: Nome vazio");
            throw new GraphQLException("O nome do produto nao pode ser vazio.").WithCode(AppErrorCodes.ValidationError);
        }

        if (price <= 0)
        {
            Log.Warning("AddProductAsync: Preco invalido: {Price}", price);
            throw new GraphQLException("O preco deve ser maior que zero.").WithCode(AppErrorCodes.ValidationError);
        }

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var categoryExists = await context.Categories.AnyAsync(c => c.Id == categoryId);

            if (!categoryExists)
            {
                Log.Warning("AddProductAsync: Categoria nao encontrada: {CategoryId}", categoryId);
                throw new GraphQLException($"Categoria com ID {categoryId} nao encontrada.").WithCode(AppErrorCodes.CategoryNotFound);
            }

            var product = new Product
            {
                Name = name.Trim(),
                Price = price,
                CategoryId = categoryId
            };

            context.Products.Add(product);
            await context.SaveChangesAsync();

            Log.Information("AddProductAsync: Sucesso | ProductId={Id}, Name={Name}", product.Id, product.Name);
            return product;
        }
        catch (GraphQLException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "AddProductAsync: Erro | Name={Name}", name);
            throw;
        }
    }

    public async Task<Product> UpdateProductAsync(int id, string? name, decimal? price)
    {
        Log.Information("UpdateProductAsync: Iniciando | Id={Id}, Name={Name}, Price={Price}", id, name, price);

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var product = await context.Products.FindAsync(id);

            if (product is null)
            {
                Log.Warning("UpdateProductAsync: Produto nao encontrado: {Id}", id);
                throw new GraphQLException($"Produto com ID {id} nao encontrado.").WithCode(AppErrorCodes.ProductNotFound);
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                product.Name = name.Trim();
            }

            if (price.HasValue)
            {
                if (price.Value <= 0)
                {
                    Log.Warning("UpdateProductAsync: Preco invalido: {Price}", price.Value);
                    throw new GraphQLException("O preco deve ser maior que zero.").WithCode(AppErrorCodes.ValidationError);
                }

                product.Price = price.Value;
            }

            await context.SaveChangesAsync();
            Log.Information("UpdateProductAsync: Sucesso | Id={Id}", id);
            return product;
        }
        catch (GraphQLException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "UpdateProductAsync: Erro | Id={Id}", id);
            throw;
        }
    }

    public async Task<Product> UpdateProductCategoryAsync(int productId, int newCategoryId)
    {
        Log.Information("UpdateProductCategoryAsync: Iniciando | ProductId={ProductId}, NewCategoryId={NewCategoryId}", productId, newCategoryId);

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var product = await context.Products.FindAsync(productId);

            if (product is null)
            {
                Log.Warning("UpdateProductCategoryAsync: Produto nao encontrado: {ProductId}", productId);
                throw new GraphQLException($"Produto com ID {productId} nao encontrado.").WithCode(AppErrorCodes.ProductNotFound);
            }

            var categoryExists = await context.Categories.AnyAsync(c => c.Id == newCategoryId);

            if (!categoryExists)
            {
                Log.Warning("UpdateProductCategoryAsync: Categoria nao encontrada: {NewCategoryId}", newCategoryId);
                throw new GraphQLException($"Categoria com ID {newCategoryId} nao encontrada.").WithCode(AppErrorCodes.CategoryNotFound);
            }

            product.CategoryId = newCategoryId;
            await context.SaveChangesAsync();

            Log.Information("UpdateProductCategoryAsync: Sucesso | ProductId={ProductId}", productId);
            return product;
        }
        catch (GraphQLException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "UpdateProductCategoryAsync: Erro | ProductId={ProductId}", productId);
            throw;
        }
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        Log.Information("DeleteProductAsync: Iniciando | Id={Id}", id);

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var product = await context.Products.FindAsync(id);

            if (product is null)
            {
                Log.Warning("DeleteProductAsync: Produto nao encontrado: {Id}", id);
                throw new GraphQLException($"Produto com ID {id} nao encontrado.").WithCode(AppErrorCodes.ProductNotFound);
            }

            context.Products.Remove(product);
            await context.SaveChangesAsync();

            Log.Information("DeleteProductAsync: Sucesso | Id={Id}", id);
            return true;
        }
        catch (GraphQLException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "DeleteProductAsync: Erro | Id={Id}", id);
            throw;
        }
    }

    public async Task<List<Product>> ApplyDiscountAsync(List<int> productIds, decimal discountPercentage)
    {
        Log.Information("ApplyDiscountAsync: Iniciando | ProductIds={ProductIds}, Discount={Discount}%", productIds, discountPercentage);

        if (discountPercentage <= 0 || discountPercentage >= 100)
        {
            Log.Warning("ApplyDiscountAsync: Desconto invalido: {Discount}", discountPercentage);
            throw new GraphQLException("O desconto deve estar entre 0 e 100%.").WithCode(AppErrorCodes.ValidationError);
        }

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var products = await context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            if (!products.Any())
            {
                Log.Warning("ApplyDiscountAsync: Nenhum produto encontrado com os IDs: {ProductIds}", productIds);
                throw new GraphQLException("Nenhum produto encontrado com os IDs fornecidos.").WithCode(AppErrorCodes.ProductNotFound);
            }

            foreach (var product in products)
            {
                product.Price -= product.Price * (discountPercentage / 100);
            }

            await context.SaveChangesAsync();

            Log.Information("ApplyDiscountAsync: Sucesso | Produtos afetados: {Count}", products.Count);
            return products;
        }
        catch (GraphQLException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "ApplyDiscountAsync: Erro | ProductIds={ProductIds}", productIds);
            throw;
        }
    }
}