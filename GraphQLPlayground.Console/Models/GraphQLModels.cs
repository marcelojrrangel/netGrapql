using Newtonsoft.Json;

namespace GraphQLPlayground.Console.Models;

public record ProductDto(int Id, string Name, decimal Price, CategoryDto? Category);
public record CategoryDto(int Id, string Name, List<ProductDto>? Products);

public class GraphQLResponse<T>
{
    public T? Data { get; set; }
    public List<GraphQLError>? Errors { get; set; }
}

public class GraphQLError
{
    public string Message { get; set; } = string.Empty;
    public string? Code { get; set; }
}

// Wrappers para garantir que o Newtonsoft mapeie corretamente o JSON do HotChocolate
public record ProductsWrapper([property: JsonProperty("products")] List<ProductDto> Products);

public record CategoryByIdWrapper([property: JsonProperty("categoryById")] CategoryDto CategoryById);

public record AddCategoryWrapper([property: JsonProperty("addCategory")] CategoryDto AddCategory);

public record AddProductWrapper([property: JsonProperty("addProduct")] ProductDto AddProduct);

public record UpdateProductWrapper([property: JsonProperty("updateProduct")] ProductDto UpdateProduct);

public record ApplyDiscountWrapper([property: JsonProperty("applyDiscount")] List<ProductDto> ApplyDiscount);