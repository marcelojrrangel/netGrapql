using System.Text;
using Newtonsoft.Json;
using Serilog;
using System.IO;
using GraphQLPlayground.Console.Services;
using GraphQLPlayground.Console.Models;

var logPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "log", "console-.log");
var graphQlUrl = Environment.GetEnvironmentVariable("GRAPHQL_API_URL") ?? "http://localhost:5200/graphql";

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "GraphQLPlayground.Console")
    .WriteTo.File(
        logPath,
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}    Properties: {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

Log.Information("Iniciando Console Client - URL: {GraphQlUrl}", graphQlUrl);

var httpClient = new HttpClient();
var client = new MyGraphQLClient(httpClient, graphQlUrl);

while (true)
{
    Console.Clear();
    Console.WriteLine("=== GraphQL Console Client Playground ===");
    Console.WriteLine("1. Listar produtos (Filtro: Preço > 100, Ordem: DESC)");
    Console.WriteLine("2. Buscar categoria por ID (ID: 1)");
    Console.WriteLine("3. Criar nova categoria (Gaming)");
    Console.WriteLine("4. Criar novo produto (PS5)");
    Console.WriteLine("5. Atualizar produto (ID: 1)");
    Console.WriteLine("6. Aplicar desconto em lote (10% nos IDs 1, 2, 3)");
    Console.WriteLine("0. Sair");
    Console.Write("\nEscolha uma opção: ");

    var choice = Console.ReadLine();

    if (choice == "0")
    {
        Log.Information("Usuario solicitou saida");
        break;
    }

    try 
    {
        switch (choice)
        {
            case "1":
                var products = await client.GetProductsAbovePrice(100);
                PrintResult("Produtos > 100", products);
                break;
            case "2":
                var category = await client.GetCategoryById(1);
                PrintResult("Categoria ID 1", category);
                break;
            case "3":
                var newCat = await client.AddCategory("Gaming");
                PrintResult("Categoria Criada", newCat);
                break;
            case "4":
                var newProd = await client.AddProduct("PS5", 499.99m, 1);
                PrintResult("Produto Criado", newProd);
                break;
            case "5":
                var updated = await client.UpdateProduct(1, "Smartphone Pro", 1199.90m);
                PrintResult("Produto Atualizado", updated);
                break;
            case "6":
                var discounted = await client.ApplyDiscount(new List<int> { 1, 2, 3 }, 10);
                PrintResult("Desconto Aplicado", discounted);
                break;
            default:
                Console.WriteLine("Opção inválida.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n[ERRO]: {ex.Message}");
        Log.Error(ex, "Erro ao processar opcao {Choice}", choice);
    }

    Console.WriteLine("\nPressione qualquer tecla para continuar...");
    Console.ReadKey();
}

void PrintResult(string title, object? data)
{
    Console.WriteLine($"\n--- {title} ---");
    Console.WriteLine(JsonConvert.SerializeObject(data, Formatting.Indented));
    Log.Information("Operacao realizada com sucesso: {Title}", title);
}

Log.Information("Console Client encerrado");
Log.CloseAndFlush();