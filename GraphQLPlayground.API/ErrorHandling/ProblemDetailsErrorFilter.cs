using System.Text.Json;
using HotChocolate;
using Microsoft.AspNetCore.Mvc;

namespace GraphQLPlayground.API.ErrorHandling;

public class ProblemDetailsErrorFilter : IErrorFilter
{
    private readonly ILogger<ProblemDetailsErrorFilter> _logger;

    public ProblemDetailsErrorFilter(ILogger<ProblemDetailsErrorFilter> logger)
    {
        _logger = logger;
    }

    public IError OnError(IError error)
    {
        var errorId = Guid.NewGuid().ToString("N")[..8];
        var errorCode = error.Extensions?["code"]?.ToString() ?? error.Code ?? "INTERNAL_ERROR";

        _logger.LogError(
            "Error {ErrorId}: {Message} | Path: {Path} | Code: {Code}",
            errorId, error.Message, error.Path, errorCode);

        if (error.Exception != null)
        {
            _logger.LogError(error.Exception, "Exception details for error {ErrorId}", errorId);
        }

        var problemDetails = new ProblemDetails
        {
            Title = GetTitle(errorCode),
            Detail = error.Message,
            Instance = $"/errors/{errorId}",
            Status = GetStatusCode(errorCode),
            Extensions = new Dictionary<string, object?>
            {
                ["errorId"] = errorId,
                ["code"] = errorCode,
                ["path"] = error.Path?.ToString(),
                ["errors"] = error.Extensions?["errors"] ?? Array.Empty<object>()
            }
        };

        return error.WithMessage(JsonSerializer.Serialize(problemDetails));
    }

    private static string GetTitle(string code) => code switch
    {
        "CATEGORY_NOT_FOUND" => "Categoria não encontrada",
        "PRODUCT_NOT_FOUND" => "Produto não encontrado",
        "VALIDATION_ERROR" => "Erro de validação",
        "CONSTRAINT_VIOLATION" => "Violação de constraint",
        _ => "Erro interno do servidor"
    };

    private static int GetStatusCode(string code) => code switch
    {
        "CATEGORY_NOT_FOUND" => 404,
        "PRODUCT_NOT_FOUND" => 404,
        "VALIDATION_ERROR" => 400,
        "CONSTRAINT_VIOLATION" => 409,
        _ => 500
    };
}