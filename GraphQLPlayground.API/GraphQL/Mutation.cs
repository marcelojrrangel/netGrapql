using GraphQLPlayground.API.Models;
using GraphQLPlayground.API.Services;

namespace GraphQLPlayground.API.GraphQL;

/*
 * No GraphQL, a Mutation é responsável por ALTERAÇÕES nos dados (Escrita, Atualização, Exclusão).
 * É o equivalente aos métodos POST, PUT, PATCH e DELETE de uma API REST clássica.
 * 
 * 🔑 CONCEITO DE MUTATIONS:
 * 
 * Enquanto as QUERIES são para LEITURA (GET), as MUTATIONS são para ESCRITA:
 * - CREATE (Criar novos registros)
 * - UPDATE (Atualizar registros existentes)
 * - DELETE (Excluir registros)
 * 
 * Diferenças importantes:
 * ✅ Mutations são executadas SEQUENCIALMENTE (uma por vez)
 * ✅ Queries são executadas em PARALELO
 * ✅ Mutations podem retornar o objeto modificado
 * ✅ Podem ter lógica de validação, regras de negócio, etc.
 */
public class Mutation
{
    // ==================== CREATE ====================

    /*
     * Mutation para criar uma categoria. 
     * Observe que retornamos a própria categoria criada - comum no GraphQL para que o cliente
     * receba os dados criados (como o ID gerado pelo banco) em uma única requisição.
     */
    public Task<Category> AddCategoryAsync(string name, [Service] ICategoryService categoryService)
        => categoryService.AddCategoryAsync(name);

    public Task<Product> AddProductAsync(
        string name,
        decimal price,
        int categoryId,
        [Service] IProductService productService)
    {
        return productService.AddProductAsync(name, price, categoryId);
    }

    // ==================== UPDATE ====================

    /*
     * Atualiza o nome e/ou preço de um produto existente.
     * Note que os parâmetros name e price são opcionais (nullable).
     * Apenas os campos fornecidos serão atualizados.
     */
    public async Task<Product> UpdateProductAsync(
        int id, 
        string? name, 
        decimal? price,
        [Service] IProductService productService)
        => await productService.UpdateProductAsync(id, name, price);

    /*
     * Atualiza a categoria de um produto.
     */
    public async Task<Product> UpdateProductCategoryAsync(
        int productId, 
        int newCategoryId, 
        [Service] IProductService productService)
        => await productService.UpdateProductCategoryAsync(productId, newCategoryId);

    public async Task<Category> UpdateCategoryAsync(
        int id, 
        string name, 
        [Service] ICategoryService categoryService)
        => await categoryService.UpdateCategoryAsync(id, name);

    // ==================== DELETE ====================

    /*
     * Exclui um produto.
     * Retorna true se foi excluído com sucesso.
     */
    public Task<bool> DeleteProductAsync(int id, [Service] IProductService productService)
        => productService.DeleteProductAsync(id);

    /*
     * Exclui uma categoria.
     * Importante: verifica se existem produtos vinculados antes de excluir.
     */
    public Task<bool> DeleteCategoryAsync(int id, [Service] ICategoryService categoryService)
        => categoryService.DeleteCategoryAsync(id);

    // ==================== OPERAÇÕES EM LOTE ====================

    /*
     * Exemplo de Mutation que aplica desconto em múltiplos produtos.
     * Útil para operações em lote.
     */
    public async Task<List<Product>> ApplyDiscountAsync(
        List<int> productIds, 
        decimal discountPercentage,
        [Service] IProductService productService)
        => await productService.ApplyDiscountAsync(productIds, discountPercentage);
}
