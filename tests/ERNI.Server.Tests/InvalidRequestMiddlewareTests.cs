using ERNI.Core;
using ERNI.Server.Middlewares;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Net;

namespace ERNI.Server.Tests;

public class InvalidRequestMiddlewareTests
{
    [Fact]
    public async Task OnInvoke_IfRequestDoesNotThrowException_Continues()
    {
        var defaultStatusCode = 420;

        //// Arrange
        RequestDelegate requestDelegate = (HttpContext _) => Task.CompletedTask;
        var sut = new InvalidRequestMiddleware(requestDelegate);
        var context = new DefaultHttpContext();
        context.Response.StatusCode = defaultStatusCode;

        //// Act
        await sut.InvokeAsync(context);

        //// Assert
        context.Response.StatusCode.Should().Be(defaultStatusCode);
    }

    [Fact]
    public async Task OnInvoke_IfRequestThrows_UnhandledException_Throws()
    {
        //// Arrange
        RequestDelegate requestDelegate = (HttpContext _) => throw new UnreachableException();
        var sut = new InvalidRequestMiddleware(requestDelegate);

        //// Act
        var ex = await Record.ExceptionAsync(() => sut.InvokeAsync(new DefaultHttpContext()));

        //// Assert
        ex.Should().BeOfType<UnreachableException>();
    }

    [Fact]
    public async Task OnInvoke_IfRequestThrows_InvalidRequestException_ResponseIsSet()
    {
        var exception = new InvalidRequestException("Invalid request", new Exception("Inner message"));

        //// Arrange
        RequestDelegate requestDelegate = (HttpContext _) => throw exception;
        var sut = new InvalidRequestMiddleware(requestDelegate);

        var context = new DefaultHttpContext();

        //// Act
        var ex = await Record.ExceptionAsync(() => sut.InvokeAsync(context));

        //// Assert
        ex.Should().BeNull();
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
    }
}
