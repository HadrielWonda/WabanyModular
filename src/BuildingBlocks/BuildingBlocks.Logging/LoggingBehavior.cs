using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Logging;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        const string prefix = nameof(LoggingBehavior<TRequest, TResponse>);

        _logger.LogInformation(
            "[{Prefix}] Handle request={X-RequestData} and response={X-ResponseData}",
            prefix,
            typeof(TRequest).Name,
            typeof(TResponse).Name);

        var timer = new Stopwatch();
        timer.Start();

        var response = await next();

        timer.Stop();
        var timeTaken = timer.Elapsed;
        if (timeTaken.Seconds > 3)
        {
            _logger.LogWarning(
                "[{Perf-Possible}] The request {X-RequestData} took {TimeTaken} seconds",
                prefix,
                typeof(TRequest).Name,
                timeTaken.Seconds);
        }

        _logger.LogInformation("[{Prefix}] Handled {X-RequestData}", prefix, typeof(TRequest).Name);
        return response;
    }
}

public class StreamLoggingBehavior<TRequest, TResponse> : IStreamPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IStreamRequest<TResponse>
    where TResponse : notnull
{
    private readonly ILogger<StreamLoggingBehavior<TRequest, TResponse>> _logger;

    public StreamLoggingBehavior(ILogger<StreamLoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public IAsyncEnumerable<TResponse> Handle(TRequest request, StreamHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        const string prefix = nameof(StreamLoggingBehavior<TRequest, TResponse>);

        _logger.LogInformation(
            "[{Prefix}] Handle request={X-RequestData} and response={X-ResponseData}",
            prefix,
            typeof(TRequest).Name,
            typeof(TResponse).Name);

        var timer = new Stopwatch();
        timer.Start();

        var response = next();

        timer.Stop();
        var timeTaken = timer.Elapsed;
        if (timeTaken.Seconds > 3)
        {
            _logger.LogWarning(
                "[{Perf-Possible}] The request {X-RequestData} took {TimeTaken} seconds",
                prefix,
                typeof(TRequest).Name,
                timeTaken.Seconds);
        }

        _logger.LogInformation("[{Prefix}] Handled {X-RequestData}", prefix, typeof(TRequest).Name);
        return response;
    }
}
