using ERNI.Core;
using ERNI.Server.Middlewares.Abstractions;
using System.Net;

namespace ERNI.Server.Middlewares;

public class UserNotFoundMiddleware : MiddlewareBase<UserNotFoundException>
{
    public UserNotFoundMiddleware(RequestDelegate next)
        : base(next)
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
}
