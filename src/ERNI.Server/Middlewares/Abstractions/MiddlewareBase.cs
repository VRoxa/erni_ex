using System.Net;

namespace ERNI.Server.Middlewares.Abstractions;

public interface IMiddleware
{
    HttpStatusCode StatusCode { get; }
}

public abstract class MiddlewareBase<TException>(RequestDelegate next) : IMiddleware
    where TException : Exception
{
    public abstract HttpStatusCode StatusCode { get; }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (TException ex)
        {
            context.Response.StatusCode = (int)StatusCode;
            await context.Response.WriteAsJsonAsync(new
            {
                ex.Message,
                Details = ex.InnerException?.Message
            });
        }
    }
}
