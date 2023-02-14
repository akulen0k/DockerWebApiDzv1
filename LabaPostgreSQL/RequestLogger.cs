namespace LabaPostgreSQL;

public class RequestLogger
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public RequestLogger(RequestDelegate next, ILoggerFactory logFactory)
    {
        _next = next;
        _logger = logFactory.CreateLogger<RequestLogger>();
    }
    
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        finally
        {
            _logger.LogInformation(
                "Request {Method} {Url} returned {StatusCode}",
                context.Request?.Method,
                context.Request?.Path.Value,
                context.Response?.StatusCode);
        }
    }
}