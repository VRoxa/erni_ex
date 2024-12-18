using ERNI.Core;
using ERNI.Server.Middlewares.Abstractions;
using System.Net;

namespace ERNI.Server.Middlewares;

public class InvalidRequestMiddleware : MiddlewareBase<InvalidRequestException>
{
    public InvalidRequestMiddleware(RequestDelegate next)
        : base(next)
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}
