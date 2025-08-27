using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace RedOps.WebApi.Middleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
    private readonly HashSet<string> _sensitiveHeaders = new(StringComparer.OrdinalIgnoreCase)
    {
        "Authorization", "Cookie", "Set-Cookie", "X-API-Key", "X-Auth-Token"
    };

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Activity.Current?.Id ?? context.TraceIdentifier;

        // Log request
        await LogRequestAsync(context, requestId);

        // Capture original response body stream
        var originalBodyStream = context.Response.Body;

        try
        {
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            // Log response
            await LogResponseAsync(context, requestId, stopwatch.ElapsedMilliseconds);

            // Copy response back to original stream
            responseBody.Position = 0;
            await responseBody.CopyToAsync(originalBodyStream);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
            stopwatch.Stop();
        }
    }

    private async Task LogRequestAsync(HttpContext context, string requestId)
    {
        try
        {
            var request = context.Request;
            var logData = new
            {
                RequestId = requestId,
                Method = request.Method,
                Path = request.Path.Value,
                QueryString = request.QueryString.Value,
                Headers = GetSafeHeaders(request.Headers),
                UserAgent = request.Headers["User-Agent"].FirstOrDefault(),
                RemoteIP = context.Connection.RemoteIpAddress?.ToString()
            };

            _logger.LogInformation("HTTP Request: {@RequestData}", logData);

            // Log request body for POST/PUT requests (if not too large)
            if (request.ContentLength > 0 && request.ContentLength < 10000 &&
                (request.Method == "POST" || request.Method == "PUT" || request.Method == "PATCH"))
            {
                request.EnableBuffering();
                var bodyContent = await ReadStreamAsync(request.Body);
                request.Body.Position = 0;

                if (!string.IsNullOrWhiteSpace(bodyContent))
                {
                    _logger.LogDebug("Request Body ({RequestId}): {RequestBody}", requestId, bodyContent);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to log request details for {RequestId}", requestId);
        }
    }

    private async Task LogResponseAsync(HttpContext context, string requestId, long elapsedMs)
    {
        try
        {
            var response = context.Response;
            var logData = new
            {
                RequestId = requestId,
                StatusCode = response.StatusCode,
                ContentType = response.ContentType,
                ContentLength = response.ContentLength,
                ElapsedMs = elapsedMs,
                Headers = GetSafeHeaders(response.Headers.ToDictionary(h => h.Key, h => h.Value))
            };

            var logLevel = response.StatusCode >= 400 ? LogLevel.Warning : LogLevel.Information;
            _logger.Log(logLevel, "HTTP Response: {@ResponseData}", logData);

            // Log response body for errors (if not too large)
            if (response.StatusCode >= 400 && response.Body is MemoryStream responseBodyStream &&
                responseBodyStream.Length > 0 && responseBodyStream.Length < 5000)
            {
                responseBodyStream.Position = 0;
                var responseContent = await ReadStreamAsync(responseBodyStream);
                responseBodyStream.Position = 0;

                if (!string.IsNullOrWhiteSpace(responseContent))
                {
                    _logger.LogDebug("Response Body ({RequestId}): {ResponseBody}", requestId, responseContent);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to log response details for {RequestId}", requestId);
        }
    }

    private Dictionary<string, string> GetSafeHeaders(IHeaderDictionary headers)
    {
        return headers
            .Where(h => !_sensitiveHeaders.Contains(h.Key))
            .ToDictionary(h => h.Key, h => string.Join(", ", h.Value.AsEnumerable()));
    }

    private Dictionary<string, string> GetSafeHeaders(Dictionary<string, StringValues> headers)
    {
        return headers
            .Where(h => !_sensitiveHeaders.Contains(h.Key))
            .ToDictionary(h => h.Key, h => string.Join(", ", h.Value.AsEnumerable()));
    }

    private static async Task<string> ReadStreamAsync(Stream stream)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
        return await reader.ReadToEndAsync();
    }
}