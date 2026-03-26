using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using MyFirstApp.Common.Constants;
using MyFirstApp.Common.Exceptions;
using MyFirstApp.Common.Models;
using MyFirstApp.Middleware;

namespace MyFirstApp.Tests.Middleware;

public sealed class ExceptionHandlingMiddlewareTests
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task InvokeAsync_ConvertsAppExceptionToStandardEnvelope()
    {
        ExceptionHandlingMiddleware middleware = new(
            _ => throw new AppException(StatusCodes.Status409Conflict, ReturnCodeConstants.RecordAlreadyExists, "Already exists"),
            NullLogger<ExceptionHandlingMiddleware>.Instance);

        DefaultHttpContext context = new();
        context.Response.Body = new MemoryStream();
        context.Request.Path = "/api/application-profiles";

        await middleware.InvokeAsync(context);

        context.Response.Body.Position = 0;
        ApiResponse<ErrorDetailDto>? payload = await JsonSerializer.DeserializeAsync<ApiResponse<ErrorDetailDto>>(context.Response.Body, SerializerOptions);

        Assert.NotNull(payload);
        Assert.Equal(StatusCodes.Status409Conflict, context.Response.StatusCode);
        Assert.Equal(ReturnCodeConstants.RecordAlreadyExists, payload.RtnCode);
        Assert.Equal("Already exists", payload.RtnMsg);
        Assert.NotNull(payload.Data);
        Assert.Equal("/api/application-profiles", payload.Data.Path);
    }

    [Fact]
    public async Task InvokeAsync_ConvertsUnhandledExceptionToSystemErrorEnvelope()
    {
        ExceptionHandlingMiddleware middleware = new(
            _ => throw new InvalidOperationException("boom"),
            NullLogger<ExceptionHandlingMiddleware>.Instance);

        DefaultHttpContext context = new();
        context.Response.Body = new MemoryStream();
        context.Request.Path = "/boom";

        await middleware.InvokeAsync(context);

        context.Response.Body.Position = 0;
        ApiResponse<ErrorDetailDto>? payload = await JsonSerializer.DeserializeAsync<ApiResponse<ErrorDetailDto>>(context.Response.Body, SerializerOptions);

        Assert.NotNull(payload);
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.Equal(ReturnCodeConstants.SystemError, payload.RtnCode);
        Assert.NotNull(payload.Data);
        Assert.Equal("/boom", payload.Data.Path);
    }
}
