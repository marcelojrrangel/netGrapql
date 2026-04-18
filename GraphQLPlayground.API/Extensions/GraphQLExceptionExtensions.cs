using HotChocolate;

namespace GraphQLPlayground.API.Extensions;

public static class GraphQLExceptionExtensions
{
    public static GraphQLException WithCode(this GraphQLException ex, string code)
    {
        ex.Data["code"] = code;
        return ex;
    }
}