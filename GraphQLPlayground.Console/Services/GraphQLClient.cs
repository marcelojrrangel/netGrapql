﻿using System.Text;
using GraphQLPlayground.Console.Models;
using Newtonsoft.Json;

namespace GraphQLPlayground.Console.Services;

public class MyGraphQLClient
{
    private readonly HttpClient _httpClient;
    private readonly string _url;

    public MyGraphQLClient(HttpClient httpClient, string url)
    {
        _httpClient = httpClient;
        _url = url;
    }

    private async Task<T?> ExecuteAsync<T>(string query, object? variables = null)
    {
        var requestBody = new { query, variables };
        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync(_url, content);
        var responseString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro Crítico no Servidor: {response.StatusCode}");

        var graphQLResponse = JsonConvert.DeserializeObject<GraphQLResponse<T>>(responseString);
        
        if (graphQLResponse?.Errors?.Any() == true)
            throw new Exception($"Erro GraphQL: {graphQLResponse.Errors.First().Message}");

        return graphQLResponse!.Data;
    }

    public async Task<List<ProductDto>> GetProductsAbovePrice(decimal minPrice)
    {
        const string query = @"
            query($price: Decimal!) { 
                products(where: { price: { gt: $price } }, order: { price: DESC }) { 
                    id name price category { name } 
                } 
            }";
        var result = await ExecuteAsync<ProductsWrapper>(query, new { price = minPrice });
        return result?.Products ?? new();
    }

    public async Task<CategoryDto?> GetCategoryById(int id)
    {
        const string query = @"
            query($id: Int!) { 
                categoryById(id: $id) { id name products { name price } } 
            }";
        var result = await ExecuteAsync<CategoryByIdWrapper>(query, new { id });
        return result?.CategoryById;
    }

    public async Task<CategoryDto> AddCategory(string name)
    {
        const string mutation = @"
            mutation($name: String!) { 
                addCategory(name: $name) { id name } 
            }";
        var result = await ExecuteAsync<AddCategoryWrapper>(mutation, new { name });
        return result!.AddCategory;
    }

    public async Task<ProductDto> AddProduct(string name, decimal price, int categoryId)
    {
        const string mutation = @"
            mutation($n: String!, $p: Decimal!, $c: Int!) { 
                addProduct(name: $n, price: $p, categoryId: $c) { id name price } 
            }";
        var result = await ExecuteAsync<AddProductWrapper>(mutation, new { n = name, p = price, c = categoryId });
        return result!.AddProduct;
    }

    public async Task<ProductDto> UpdateProduct(int id, string name, decimal price)
    {
        const string mutation = @"
            mutation($id: Int!, $n: String, $p: Decimal) { 
                updateProduct(id: $id, name: $n, price: $p) { id name price } 
            }";
        var result = await ExecuteAsync<UpdateProductWrapper>(mutation, new { id, n = name, p = price });
        return result!.UpdateProduct;
    }

    public async Task<List<ProductDto>> ApplyDiscount(List<int> ids, decimal percentage)
    {
        const string mutation = @"
            mutation($ids: [Int!]!, $perc: Decimal!) { 
                applyDiscount(productIds: $ids, discountPercentage: $perc) { id name price } 
            }";
        var result = await ExecuteAsync<ApplyDiscountWrapper>(mutation, new { ids, perc = percentage });
        return result!.ApplyDiscount;
    }
}