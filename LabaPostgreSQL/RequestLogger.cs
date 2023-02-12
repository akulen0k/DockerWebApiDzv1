using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;


namespace LabaPostgreSQL;


public class RequestLogger
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public RequestLogger(RequestDelegate next, ILoggerFactory logfact)
    {
        _next = next;
        _logger = logfact.CreateLogger<RequestLogger>();
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
                "Request {method} {url} returned {statusCode}",
                context.Request?.Method,
                context.Request?.Path.Value,
                context.Response?.StatusCode);
        }
    }
}