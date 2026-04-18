using GraphQLPlayground.API.Data;
using GraphQLPlayground.API.ErrorHandling;
using GraphQLPlayground.API.GraphQL;
using GraphQLPlayground.API.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var logDirectory = System.IO.Path.Combine(builder.Environment.ContentRootPath, "log");
Directory.CreateDirectory(logDirectory);
var apiLogPath = System.IO.Path.Combine(logDirectory, "api-.log");

// Configuração do Serilog para API - logs por dia em pasta "log" + envio para Loki
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "GraphQLPlayground.API")
    .Enrich.WithProperty("Environment", "development")
    .WriteTo.File(
        apiLogPath,
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}    Properties: {Properties:j}{NewLine}{Exception}")
    .WriteTo.Http(
        "http://localhost:3100/loki/api/v1/push",
        queueLimitBytes: null)
    .CreateLogger();

Log.Information("Iniciando aplicacao GraphQLPlayground.API...");

// 1. Configuração do PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                    ?? "Host=localhost;Database=graphql_db;Username=user;Password=password";

/*
 * Para GraphQL, é recomendado usar o Pool de DbContext para suportar a execução
 * paralela de múltiplos campos em uma única query sem conflitos de concorrência.
 */
builder.Services.AddPooledDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// 1.1 Registra camada de services (regras de negocio)
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();

// 2. Configuração do GraphQL Server (HotChocolate)
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddProjections()
    .AddFiltering()
    .AddSorting()
    .RegisterDbContextFactory<AppDbContext>()
    .AddErrorFilter<ProblemDetailsErrorFilter>();

var app = builder.Build();

// Garantir que o banco seja criado (apenas para este exemplo rápido)
using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    using var db = factory.CreateDbContext();
    db.Database.EnsureCreated();
}

// 3. Mapeia o endpoint do GraphQL (por padrão: /graphql)
// O HotChocolate também fornece o "Banana Cake Pop", uma interface visual para testar queries.
app.MapGraphQL();

app.MapGet("/", () => Results.Redirect("/graphql"));

app.Run();
