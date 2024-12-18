using ERNI.Core;
using ERNI.Server.Middlewares.Abstractions;
using System.Net;

namespace ERNI.Server.Middlewares;

public class OrderNotFoundMiddleware : MiddlewareBase<OrderNotFoundException>
{
    public OrderNotFoundMiddleware(RequestDelegate next)
        : base(next)
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
}
