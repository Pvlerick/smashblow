using Microsoft.AspNetCore.HttpLogging;
using System.Diagnostics;

var stopwatch = Stopwatch.StartNew();

var random = new Random();
var content = new byte[] {};

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpLogging(o => o.LoggingFields = HttpLoggingFields.All);

var app = builder.Build();
app.UseHttpLogging();
app.UseMiddleware<CustomLoggingMiddleware>();

app.MapGet("/", () => "Lorem ipsum etc...");
app.MapGet("/delay/{seconds}", async Task<IResult> (int seconds) =>
{
    await Task.Delay(TimeSpan.FromSeconds(seconds));
    return TypedResults.Ok("That was slow!");
});
app.MapPost("/shutdown", async () =>
{
    await Task.Delay(TimeSpan.FromSeconds(2));
    await app.StopAsync();
    return TypedResults.NoContent();
});
app.MapPost("/crash", () => Environment.Exit(-1));

app.MapGet("/elapsed/{seconds}", IResult (int seconds) => stopwatch.Elapsed >= TimeSpan.FromSeconds(seconds) ? (IResult)TypedResults.NoContent() : TypedResults.StatusCode(500));

app.MapPost("/size/{mb}", IResult (int mb) => 
{
    content = new byte[mb * 1000_000];
    random.NextBytes(content);
    GC.Collect(2);
    return (IResult)TypedResults.NoContent();
});

app.Run("http://0.0.0.0:8000");

class CustomLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public CustomLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next;
        _logger = loggerFactory.CreateLogger<CustomLoggingMiddleware>();
    }

    public async Task Invoke(HttpContext context)
    {
        _logger.LogInformation(
            "Received request from {src}/{srcprt} targeting {dst}/{dstprt}",
            context.Connection.RemoteIpAddress?.MapToIPv4(),
            context.Connection.RemotePort,
            context.Connection.LocalIpAddress?.MapToIPv4(),
            context.Connection.LocalPort
        );
        
        await _next(context);
    }
}
