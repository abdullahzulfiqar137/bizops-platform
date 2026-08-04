using BusinessOperations.API.Middleware;

namespace BusinessOperations.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseCustomExceptionMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionMiddleware>();
    }
}
