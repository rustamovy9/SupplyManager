using System.Diagnostics;

namespace MainApp.Middleware;

public class CustomMiddleware
{
    private readonly RequestDelegate _next;
    public CustomMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    private int _requestCount = 0;
    private const int LimitRequest = 5;
    private DateTime _resetTime = DateTime.UtcNow.AddSeconds(5);
    public async Task InvokeAsync(HttpContext context)
    {
        Stopwatch watch = Stopwatch.StartNew();
        PathString requestPath = context.Request.Path;
        string requestMethod = context.Request.Method;
       
        if (DateTime.UtcNow > _resetTime)
        {
            _requestCount = 0; 
            _resetTime = DateTime.UtcNow.AddSeconds(5); 
        }
        else
        {
            _requestCount++;
        }

        if (_requestCount > LimitRequest)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;  
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync("Request limit exceeded. Please try again later.");
            return;
        }
        
        Console.WriteLine($"Incoming Request: {requestMethod} {requestPath}");
        await _next(context);
        watch.Stop();
        var responseStatusCode = context.Response.StatusCode;
        Console.WriteLine($"Outgoing Reply: {responseStatusCode} for {requestMethod} {requestPath} - During this time - {watch.ElapsedMilliseconds} ms");
    }
}
